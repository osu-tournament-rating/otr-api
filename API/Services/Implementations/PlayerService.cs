using API.DTOs;
using API.Entities;
using API.Osu;
using API.Services.Interfaces;
using API.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Npgsql.Internal.TypeHandlers;

namespace API.Services.Implementations;

public class PlayerService : ServiceBase<Player>, IPlayerService
{
	private readonly OtrContext _context;
	private readonly IMapper _mapper;
	private readonly IServiceProvider _serviceProvider;

	public PlayerService(ILogger<PlayerService> logger, OtrContext context, IServiceProvider serviceProvider, IMapper mapper) : base(logger, context)
	{
		_context = context;
		_serviceProvider = serviceProvider;
		_mapper = mapper;
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

	public async Task<IEnumerable<PlayerDTO>> GetAllAsync() => _mapper.Map<IEnumerable<PlayerDTO>>(await _context.Players
	                                                                                                             .Include(x => x.MatchScores)
	                                                                                                             .Include(x => x.RatingHistories)
	                                                                                                             .Include(x => x.Ratings)
	                                                                                                             .ToListAsync());

	public async Task<Player?> GetPlayerByOsuIdAsync(long osuId, bool eagerLoad = false, int mode = 0, int offsetDays = -1)
	{
		if (!eagerLoad)
		{
			return await _context.Players.Where(x => x.OsuId == osuId).FirstOrDefaultAsync();
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

	public async Task<IEnumerable<PlayerDTO>> GetByOsuIdsAsync(IEnumerable<long> osuIds) =>
		_mapper.Map<IEnumerable<PlayerDTO>>(await _context.Players.Where(p => osuIds.Contains(p.OsuId)).ToListAsync());

	public async Task<int> GetIdByOsuIdAsync(long osuId) => await _context.Players.Where(p => p.OsuId == osuId).Select(p => p.Id).FirstOrDefaultAsync();
	public async Task<long> GetOsuIdByIdAsync(int id) => await _context.Players.Where(p => p.Id == id).Select(p => p.OsuId).FirstOrDefaultAsync();
	public async Task<IEnumerable<PlayerRanksDTO>> GetAllRanksAsync() => _mapper.Map<IEnumerable<PlayerRanksDTO>>(await _context.Players.ToListAsync());

	public async Task<IEnumerable<Unmapped_PlayerRatingDTO>> GetTopRatingsAsync(int n, OsuEnums.Mode mode) => await (from p in _context.Players
	                                                                                                                 join r in _context.Ratings on p.Id equals r.PlayerId
	                                                                                                                 where r.Mode == (int)mode
	                                                                                                                 orderby r.Mu descending
	                                                                                                                 select new Unmapped_PlayerRatingDTO
	                                                                                                                 {
		                                                                                                                 OsuId = p.OsuId,
		                                                                                                                 Username = p.Username,
		                                                                                                                 Mu = r.Mu,
		                                                                                                                 Sigma = r.Sigma
	                                                                                                                 })
	                                                                                                                .Take(n)
	                                                                                                                .ToListAsync();

	public async Task<Unmapped_PlayerStatisticsDTO> GetVerifiedPlayerStatisticsAsync(long osuId, OsuEnums.Mode mode, DateTime? fromPointInTime = null)
	{
		int modeInt = (int)mode;
		var stats = new Unmapped_PlayerStatisticsDTO();

		var time = fromPointInTime ?? DateTime.MinValue;

		using (var scope = _serviceProvider.CreateScope())
		{
			var ratingsService = scope.ServiceProvider.GetRequiredService<IRatingsService>();
			var matchesService = scope.ServiceProvider.GetRequiredService<IMatchesService>();
			var gamesService = scope.ServiceProvider.GetRequiredService<IGamesService>();

			int offsetDays = (int)DateTime.UtcNow.Subtract(fromPointInTime ?? DateTime.MinValue).TotalDays;
			var player = await GetPlayerByOsuIdAsync(osuId, true, offsetDays);
			if (player == null)
			{
				return stats;
			}

			var rating = await _context.Ratings
			                           .WherePlayer(player.OsuId)
			                           .WhereMode(modeInt)
			                           .FirstOrDefaultAsync();
			
			if (rating != null)
			{
				stats.Rating = (int)rating.Mu;
				stats.RatingDelta = RatingUtils.GetRatingDelta(stats.Rating);
				stats.RatingForNextRank = RatingUtils.GetRatingNeededForNextRank(stats.Rating);
				stats.Ranking = RatingUtils.GetRankingClassName(stats.Rating);
				stats.NextRanking = RatingUtils.GetNextRankingClassName(stats.Rating);

				var ratingHistories = await _context.RatingHistories
				                                    .WherePlayer(player.OsuId)
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

				stats.CountryRank = (await _context.Ratings
				                                   .Include(x => x.Player)
				                                   .Where(x => x.Player.Country == player.Country)
				                                   .WhereMode(modeInt)
				                                   .OrderByMuDescending()
				                                   .ToListAsync())
				                    .TakeWhile(x => x.PlayerId != player.Id)
				                    .Count() + 1;

				// Add 1 to represent rank, as this is an index
				stats.GlobalRank = (await _context.Ratings.WhereMode(modeInt).OrderByMuDescending().ToListAsync()).TakeWhile(x => x.PlayerId != player.Id).Count() + 1;

				stats.Percentile = 100 *
				                   (1 -
				                    (((await _context.Ratings.WhereMode(modeInt).OrderByMuDescending().ToListAsync()).TakeWhile(x => x.PlayerId != player.Id).Count() + 1) /
				                     (double)await _context.Ratings.WhereMode(modeInt).CountAsync()));


				stats.MatchesPlayed = await _context.MatchScores
				                                    .WhereVerified()
				                                    .WherePlayer(osuId)
				                                    .WhereMode(modeInt)
				                                    .After(time)
				                                    .Select(x => x.Game.Match)
				                                    .Distinct()
				                                    .CountAsync();

				stats.GamesPlayed = await _context.MatchScores
				                                  .WhereVerified()
				                                  .WherePlayer(player.OsuId)
				                                  .WhereMode(modeInt)
				                                  .After(time)
				                                  .Select(x => x.GameId).Distinct().CountAsync();

				stats.MatchesWon = await matchesService.CountMatchWinsAsync(osuId, modeInt, time);
				stats.GamesWon = await gamesService.CountGameWinsAsync(osuId, modeInt, time);
				
				stats.GamesLost = stats.GamesPlayed - stats.GamesWon;
				stats.MatchesLost = stats.MatchesPlayed - stats.MatchesWon;

				stats.AverageOpponentRating = await ratingsService.AverageOpponentRating(osuId, modeInt);
				stats.AverageTeammateRating = await ratingsService.AverageTeammateRating(osuId, modeInt);
				
				stats.PlayedHR = await _context.MatchScores
				                              .WhereVerified()
				                              .WherePlayer(osuId)
				                              .WhereMode(modeInt)
				                              .WhereMods(OsuEnums.Mods.HardRock)
				                              .After(time)
				                              .CountAsync();
				
				stats.PlayedHD = await _context.MatchScores
				                              .WhereVerified()
				                              .WherePlayer(osuId)
				                              .WhereMode(modeInt)
				                              .WhereMods(OsuEnums.Mods.Hidden)
				                              .After(time)
				                              .CountAsync();
				
				stats.PlayedDT = await _context.MatchScores
				                              .WhereVerified()
				                              .WherePlayer(osuId)
				                              .WhereMode(modeInt)
				                              .WhereMods(OsuEnums.Mods.DoubleTime)
				                              .After(time)
				                              .CountAsync();
				
				// Trend
				stats.IsRatingPositiveTrend = await ratingsService.IsRatingPositiveTrendAsync(osuId, modeInt, time);
			}
		}

		return stats;
	}

	// This is used by a scheduled task to automatically populate user info, such as username, country, etc.
	public async Task<IEnumerable<Player>> GetOutdatedAsync() => await _context.Players.Where(p => p.Updated == null).ToListAsync();
}