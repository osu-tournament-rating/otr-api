using API.DTOs;
using API.Entities;
using API.Osu;
using API.Repositories.Interfaces;
using API.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class PlayerRepository : RepositoryBase<Player>, IPlayerRepository
{
	private readonly OtrContext _context;
	private readonly IMapper _mapper;
	private readonly IServiceProvider _serviceProvider;

	public PlayerRepository(OtrContext context, IServiceProvider serviceProvider, IMapper mapper) : base(context)
	{
		_context = context;
		_serviceProvider = serviceProvider;
		_mapper = mapper;
	}

	public override async Task<Player?> CreateAsync(Player player)
	{
		player.Created = DateTime.UtcNow;
		return await base.CreateAsync(player);
	}

	public async Task<IEnumerable<Player>> GetPlayersWhereMissingGlobalRankAsync()
	{
		// Get all players that are missing an earliest global rank in any mode (but have a current rank in that mode)
		var players = await _context.Players.Where(x => (x.EarliestOsuGlobalRank == null && x.RankStandard != null) ||
		                                                (x.EarliestTaikoGlobalRank == null && x.RankTaiko != null) ||
		                                                (x.EarliestCatchGlobalRank == null && x.RankCatch != null) ||
		                                                (x.EarliestManiaGlobalRank == null && x.RankMania != null))
		                            .ToListAsync();

		return players;
	}

	public async Task<IEnumerable<Player>> GetByOsuIdsAsync(IEnumerable<long> osuIds) => await _context.Players.Where(p => osuIds.Contains(p.OsuId)).ToListAsync();

	public async Task<IEnumerable<Player>> GetAllAsync() => await _context.Players
	                                                                      .Include(x => x.MatchScores)
	                                                                      .Include(x => x.RatingHistories)
	                                                                      .Include(x => x.Ratings)
	                                                                      .ToListAsync();

	public async Task<Player?> GetPlayerByOsuIdAsync(long osuId, bool eagerLoad = false, int mode = 0, int offsetDays = -1)
	{
		if (!eagerLoad)
		{
			return await _context.Players.WhereOsuId(osuId).FirstOrDefaultAsync();
		}

		var time = offsetDays == -1 ? DateTime.MinValue : DateTime.UtcNow.AddDays(-offsetDays);

		var p = await _context.Players
		                      .Include(x => x.MatchScores.Where(y => y.Game.StartTime > time && y.Game.PlayMode == mode))
		                      .ThenInclude(x => x.Game)
		                      .Include(x => x.RatingHistories.Where(y => y.Created > time && y.Mode == mode))
		                      .ThenInclude(x => x.Match)
		                      .Include(x => x.Ratings.Where(y => y.Mode == mode))
		                      .Include(x => x.User)
		                      .WhereOsuId(osuId)
		                      .FirstOrDefaultAsync();

		if (p == null)
		{
			return null;
		}

		return p;
	}

	public async Task<PlayerDTO?> GetPlayerDTOByOsuIdAsync(long osuId, bool eagerLoad = false, OsuEnums.Mode mode = OsuEnums.Mode.Standard, int offsetDays = -1)
	{
		var time = offsetDays == -1 ? DateTime.MinValue : DateTime.UtcNow.AddDays(-offsetDays);
		var obj = _mapper.Map<PlayerDTO?>(await GetPlayerByOsuIdAsync(osuId, eagerLoad, (int)mode, offsetDays));

		if (obj == null)
		{
			return obj;
		}

		obj.Statistics = await GetVerifiedPlayerStatisticsAsync(osuId, mode, time);
		return obj;
	}

	

	public async Task<int> GetIdByOsuIdAsync(long osuId) => await _context.Players.Where(p => p.OsuId == osuId).Select(p => p.Id).FirstOrDefaultAsync();
	public async Task<long> GetOsuIdByIdAsync(int id) => await _context.Players.Where(p => p.Id == id).Select(p => p.OsuId).FirstOrDefaultAsync();

	public async Task<IEnumerable<PlayerRatingDTO>> GetTopRatingsAsync(int n, OsuEnums.Mode mode) => await (from p in _context.Players
	                                                                                                                 join r in _context.Ratings on p.Id equals r.PlayerId
	                                                                                                                 where r.Mode == (int)mode
	                                                                                                                 orderby r.Mu descending
	                                                                                                                 select new PlayerRatingDTO
	                                                                                                                 {
		                                                                                                                 OsuId = p.OsuId,
		                                                                                                                 Username = p.Username,
		                                                                                                                 Mu = r.Mu,
		                                                                                                                 Sigma = r.Sigma
	                                                                                                                 })
	                                                                                                                .Take(n)
	                                                                                                                .ToListAsync();

	public async Task<string?> GetUsernameAsync(long osuId) => await _context.Players.WhereOsuId(osuId).Select(p => p.Username).FirstOrDefaultAsync();

	public async Task<Unmapped_PlayerStatisticsDTO> GetVerifiedPlayerStatisticsAsync(long osuId, OsuEnums.Mode mode, DateTime? fromPointInTime = null)
	{
		int modeInt = (int)mode;
		var stats = new Unmapped_PlayerStatisticsDTO
		{
			OsuId = osuId,
			Created = DateTime.UtcNow
		};

		var time = fromPointInTime ?? DateTime.MinValue;

		using (var scope = _serviceProvider.CreateScope())
		{
			var ratingsService = scope.ServiceProvider.GetRequiredService<IRatingsRepository>();
			var matchesService = scope.ServiceProvider.GetRequiredService<IMatchesRepository>();
			var gamesService = scope.ServiceProvider.GetRequiredService<IGamesRepository>();

			int offsetDays = (int)DateTime.UtcNow.Subtract(fromPointInTime ?? DateTime.MinValue).TotalDays;
			var player = await GetPlayerByOsuIdAsync(osuId, true, offsetDays);
			if (player == null)
			{
				return stats;
			}

			var rating = await _context.Ratings
			                           .WhereOsuPlayerId(player.OsuId)
			                           .WhereMode(modeInt)
			                           .FirstOrDefaultAsync();

			if (rating == null)
			{
				return stats;
			}

			stats.Rating = (int)rating.Mu;
			stats.RatingDelta = RatingUtils.GetRatingDelta(stats.Rating);
			stats.RatingForNextRank = RatingUtils.GetRatingForNextTier(stats.Rating);
			stats.Ranking = RatingUtils.GetTier(stats.Rating);
			stats.NextRanking = RatingUtils.GetNextTier(stats.Rating);

			var ratingHistories = await _context.RatingHistories
			                                    .WhereOsuPlayerId(player.OsuId)
			                                    .WhereMode(modeInt)
			                                    .After(time)
			                                    .OrderBy(x => x.Created)
			                                    .ToListAsync();

			if (ratingHistories.Any())
			{
				stats.HighestRating = (int)ratingHistories.Max(r => r.Mu);
				int? ratingGainedSincePeriod = stats.Rating - (int?)player.RatingHistories.FirstOrDefault(x => x.Mode == modeInt)?.Mu;

				stats.RatingGainedSincePeriod = ratingGainedSincePeriod ?? 0;
			}

			int countryIndex = _context.Ratings
			                           .Where(x => x.Player.Country == player.Country)
			                           .WhereMode(modeInt)
			                           .OrderByMuDescending()
			                           .Select(x => x.PlayerId)
			                           .AsEnumerable() // Transfer the execution to client-side after this point
			                           .TakeWhile(x => x != player.Id)
			                           .Count();

			stats.CountryRank = countryIndex + 1;

			stats.GlobalRank = await ratingsService.GetGlobalRankAsync(osuId, modeInt);

			int totalRatings = await _context.Ratings
			                                 .WhereMode(modeInt)
			                                 .CountAsync();

			stats.Percentile = 100 * (1 - (stats.GlobalRank / (double)totalRatings));

			stats.MatchesPlayed = await matchesService.CountMatchesPlayedAsync(osuId, modeInt, time);

			stats.GamesPlayed = await _context.MatchScores
			                                  .WhereVerified()
			                                  .WhereOsuPlayerId(player.OsuId)
			                                  .WhereMode(modeInt)
			                                  .After(time)
			                                  .Select(x => x.GameId)
			                                  .Distinct()
			                                  .CountAsync();

			stats.MatchesWon = await matchesService.CountMatchWinsAsync(osuId, modeInt, time);
			stats.GamesWon = await gamesService.CountGameWinsAsync(osuId, modeInt, time);

			stats.GamesLost = stats.GamesPlayed - stats.GamesWon;
			stats.MatchesLost = stats.MatchesPlayed - stats.MatchesWon;

			stats.AverageOpponentRating = await ratingsService.AverageOpponentRating(osuId, modeInt);
			stats.AverageTeammateRating = await ratingsService.AverageTeammateRating(osuId, modeInt);

			stats.PlayedHR = await _context.MatchScores
			                               .WhereVerified()
			                               .WhereOsuPlayerId(osuId)
			                               .WhereMode(modeInt)
			                               .WhereMods(OsuEnums.Mods.HardRock)
			                               .After(time)
			                               .CountAsync();

			stats.PlayedHD = await _context.MatchScores
			                               .WhereVerified()
			                               .WhereOsuPlayerId(osuId)
			                               .WhereMode(modeInt)
			                               .WhereMods(OsuEnums.Mods.Hidden)
			                               .After(time)
			                               .CountAsync();

			stats.PlayedDT = await _context.MatchScores
			                               .WhereVerified()
			                               .WhereOsuPlayerId(osuId)
			                               .WhereMode(modeInt)
			                               .WhereMods(OsuEnums.Mods.DoubleTime)
			                               .After(time)
			                               .CountAsync();

			stats.MostPlayedTeammate = await gamesService.MostPlayedTeammateNameAsync(osuId, modeInt, time);
			stats.MostPlayedOpponent = await gamesService.MostPlayedOpponentNameAsync(osuId, modeInt, time);

			stats.BestPerformingTeammate = await ratingsService.BestPerformingTeammateNameAsync(osuId, modeInt, time);
			stats.WorstPerformingTeammate = await ratingsService.WorstPerformingTeammateNameAsync(osuId, modeInt, time);
			stats.BestPerformingOpponent = await ratingsService.BestPerformingOpponentNameAsync(osuId, modeInt, time);
			stats.WorstPerformingOpponent = await ratingsService.WorstPerformingOpponentNameAsync(osuId, modeInt, time);
			
			// Trend
			stats.IsRatingPositiveTrend = await ratingsService.IsRatingPositiveTrendAsync(osuId, modeInt, time);
		}

		return stats;
	}

	// This is used by a scheduled task to automatically populate user info, such as username, country, etc.
	public async Task<IEnumerable<Player>> GetOutdatedAsync() => await _context.Players.Where(p => p.Updated == null).ToListAsync();
}