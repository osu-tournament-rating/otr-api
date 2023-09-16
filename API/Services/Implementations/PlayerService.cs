using API.DTOs;
using API.Entities;
using API.Osu;
using API.Services.Interfaces;
using API.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class PlayerService : ServiceBase<Player>, IPlayerService
{
	private readonly OtrContext _context;
	private readonly IServiceProvider _serviceProvider;
	private readonly IMapper _mapper;

	public PlayerService(ILogger<PlayerService> logger, OtrContext context, IServiceProvider serviceProvider, IMapper mapper) : base(logger, context)
	{
		_context = context;
		_serviceProvider = serviceProvider;
		_mapper = mapper;
	}

	public async Task<IEnumerable<PlayerDTO>> GetAllAsync() => _mapper.Map<IEnumerable<PlayerDTO>>(await _context.Players
	                                                                                                             .Include(x => x.MatchScores)
	                                                                                                             .Include(x => x.RatingHistories)
	                                                                                                             .Include(x => x.Ratings)
	                                                                                                             .ToListAsync());

	public async Task<Player?> GetPlayerByOsuIdAsync(long osuId, bool eagerLoad = false)
	{
		if (!eagerLoad)
		{
			return await _context.Players.Where(x => x.OsuId == osuId).FirstOrDefaultAsync();
		}

		return await _context.Players
		                     .Include(x => x.MatchScores)
		                     .Include(x => x.RatingHistories)
		                     .Include(x => x.Ratings)
		                     .Include(x => x.User)
		                     .Where(x => x.OsuId == osuId)
		                     .FirstOrDefaultAsync();
	}

	public async Task<PlayerDTO?> GetPlayerDTOByOsuIdAsync(long osuId, bool eagerLoad = false)
	{
		var obj = _mapper.Map<PlayerDTO?>(await GetPlayerByOsuIdAsync(osuId, eagerLoad));

		if (obj == null)
		{
			return obj;
		}

		obj.Statistics = await GetPlayerStatisticsAsync(osuId, OsuEnums.Mode.Standard);
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

	public async Task<Unmapped_PlayerStatisticsDTO> GetPlayerStatisticsAsync(long osuId, OsuEnums.Mode mode, DateTime? fromPointInTime = null)
	{
		int modeInt = (int)mode;
		var stats = new Unmapped_PlayerStatisticsDTO();

		var time = fromPointInTime ?? DateTime.MinValue;
		
		using (var scope = _serviceProvider.CreateScope())
		{
			var ratingsService = scope.ServiceProvider.GetRequiredService<IRatingsService>();
			var matchesService = scope.ServiceProvider.GetRequiredService<IMatchesService>();
			var gamesService = scope.ServiceProvider.GetRequiredService<IGamesService>();
			
			var player = await _context.Players.FirstOrDefaultAsync(p => p.OsuId == osuId);
			if (player == null)
			{
				return stats;
			}

			var rating = await _context.Ratings.Where(r => r.PlayerId == player.Id && r.Mode == modeInt).FirstOrDefaultAsync();
			if (rating != null)
			{
				stats.Rating = (int)rating.Mu;
				stats.RatingDelta = RatingUtils.GetRatingDelta(stats.Rating);
				stats.RatingForNextRank = RatingUtils.GetRatingNeededForNextRank(stats.Rating);
				stats.Ranking = RatingUtils.GetRankingClassName(stats.Rating);
				stats.NextRanking = RatingUtils.GetNextRankingClassName(stats.Rating);

				var ratingHistories = await _context.RatingHistories.Where(r => r.PlayerId == player.Id && r.Mode == modeInt && r.Created >= time).OrderBy(x => x.Created).ToListAsync();
				if (ratingHistories.Any())
				{
					stats.HighestRating = (int)ratingHistories.Max(r => r.Mu);
				}

				stats.CountryRank = (await _context.Ratings.Where(x => x.Player.Country == player.Country)
				                                  .OrderByMuDescending()
				                                  .ToListAsync())
				                                  .TakeWhile(x => x.PlayerId != player.Id && x.Mode != modeInt)
				                                  .Count() + 1;

				// Add 1 to represent rank, as this is an index
				stats.GlobalRank = (await _context.Ratings.WhereMode(modeInt).OrderByMuDescending().ToListAsync()).TakeWhile(x => x.PlayerId != player.Id).Count() + 1;
				stats.HighestGlobalRank = await _context.RatingHistories.WherePlayer(osuId)
				                                        .OrderByMuDescending()
				                                        .Take(1)
				                                        .Select(x => x.GlobalRank)
				                                        .FirstOrDefaultAsync() + 1;

				stats.Percentile = 100 * (1 - (((await _context.Ratings.WhereMode(modeInt).OrderByMuDescending().ToListAsync()).TakeWhile(x => x.PlayerId != player.Id).Count() + 1) /
				                    (double)await _context.Ratings.WhereMode(modeInt).CountAsync()));

				stats.HighestPercentile = 100 *
					(await _context.RatingHistories.WherePlayer(osuId).OrderByMuDescending().Take(1).Select(x => x.GlobalRank).FirstOrDefaultAsync() /
						(double)await _context.Ratings.Where(x => x.Mode == modeInt).CountAsync() + 1);

				stats.MatchesPlayed = await _context.MatchScores
				                                    .WherePlayer(osuId)
				                                    .WhereMode(modeInt)
				                                    .Select(x => x.Game.Match)
				                                    .Distinct()
				                                    .CountAsync();

				stats.GamesPlayed = await _context.MatchScores.Where(x => x.PlayerId == player.Id && x.Game.PlayMode == modeInt).Select(x => x.GameId).Distinct().CountAsync();

				stats.GamesWon = await gamesService.CountGameWinsAsync(osuId, modeInt);
				stats.MatchesWon = await matchesService.CountMatchWinsAsync(osuId, modeInt);

				stats.AverageOpponentRating = await ratingsService.AverageOpponentRating(osuId, modeInt);
				stats.AverageTeammateRating = await ratingsService.AverageTeammateRating(osuId, modeInt);
			}
		}
		
		return stats;
	}

	// This is used by a scheduled task to automatically populate user info, such as username, country, etc.
	public async Task<IEnumerable<Player>> GetOutdatedAsync() => await _context.Players.Where(p => p.Updated == null).ToListAsync();
}