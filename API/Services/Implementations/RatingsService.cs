using API.DTOs;
using API.Entities;
using API.Services.Interfaces;
using API.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class RatingsService : ServiceBase<Rating>, IRatingsService
{
	private readonly OtrContext _context;
	private readonly ILogger<RatingsService> _logger;
	private readonly IMapper _mapper;

	public RatingsService(ILogger<RatingsService> logger, IMapper mapper, OtrContext context) : base(logger, context)
	{
		_logger = logger;
		_mapper = mapper;
		_context = context;
	}

	public async Task<IEnumerable<RatingDTO>> GetForPlayerAsync(long osuPlayerId)
	{
		int dbId = await _context.Players.Where(x => x.OsuId == osuPlayerId).Select(x => x.Id).FirstOrDefaultAsync();

		if (dbId == 0)
		{
			return new List<RatingDTO>();
		}

		return _mapper.Map<IEnumerable<RatingDTO>>(await _context.Ratings.Where(x => x.PlayerId == dbId).ToListAsync());
	}

	public override async Task<int> UpdateAsync(Rating entity)
	{
		// First, copy the current state of the entity to the history table.
		var history = new RatingHistory
		{
			PlayerId = entity.PlayerId,
			Mu = entity.Mu,
			Sigma = entity.Sigma,
			Created = DateTime.UtcNow,
			Mode = entity.Mode
		};

		await _context.RatingHistories.AddAsync(history);
		return await base.UpdateAsync(entity);
	}

	public async Task<int> InsertOrUpdateForPlayerAsync(int playerId, Rating rating)
	{
		var existingRating = await _context.Ratings
		                                   .Where(r => r.PlayerId == rating.PlayerId && r.Mode == rating.Mode)
		                                   .FirstOrDefaultAsync();

		if (existingRating != null)
		{
			existingRating.Mu = rating.Mu;
			existingRating.Sigma = rating.Sigma;
			existingRating.Updated = rating.Updated;
		}
		else
		{
			_context.Ratings.Add(rating);
		}

		return await _context.SaveChangesAsync();
	}

	public async Task<int> BatchInsertAsync(IEnumerable<RatingDTO> ratings)
	{
		var ls = new List<Rating>();
		foreach (var rating in ratings)
		{
			ls.Add(new Rating
			{
				PlayerId = rating.PlayerId,
				Mu = rating.Mu,
				Sigma = rating.Sigma,
				Created = DateTime.UtcNow,
				Mode = rating.Mode
			});
		}

		await _context.Ratings.AddRangeAsync(ls);
		return await _context.SaveChangesAsync();
	}

	public async Task<IEnumerable<RatingDTO>> GetAllAsync() => _mapper.Map<IEnumerable<RatingDTO>>(await _context.Ratings.ToListAsync());
	public async Task TruncateAsync() => await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE ratings RESTART IDENTITY;");

	public async Task<int> GetGlobalRankAsync(long osuPlayerId, int mode)
	{
		int globalIndex = (await _context.Ratings
		                          .WhereMode(mode)
		                          .OrderByMuDescending()
		                          .Select(x => x.Player.OsuId)
		                          .ToListAsync())
		                          .TakeWhile(x => x != osuPlayerId)
		                          .Count();

		return globalIndex + 1;
	}

	public async Task<int> AverageTeammateRating(long osuPlayerId, int mode)
	{
		double averageRating = await _context.MatchScores
		                                     .WhereVerified()
		                                     .WhereNotHeadToHead()
		                                     .WhereTeammate(osuPlayerId)
		                                     .SelectMany(x => x.Player.Ratings)
		                                     .Where(rating => rating != null && rating.Mode == mode)
		                                     .AverageAsync(rating => (double?)rating.Mu) ??
		                       0.0;

		return (int)averageRating;
	}

	public async Task<int> AverageOpponentRating(long osuPlayerId, int mode)
	{
		// Define a query to select MatchScores with verified status
		var verifiedMatchScores = _context.MatchScores.WhereVerified();

		// Filter MatchScores to only those where the opponent is the given player
		var opponentMatchScores = verifiedMatchScores.WhereOpponent(osuPlayerId);

		// Project to the player IDs instead of including whole player entities
		var opponentPlayerIds = await opponentMatchScores.Select(x => x.PlayerId).ToListAsync();

		// Use the player IDs to filter the Ratings directly, avoiding unnecessary Includes
		var opponentRatings = _context.Ratings
		                              .WhereMode(mode)
		                              .Where(x => opponentPlayerIds.Contains(x.PlayerId));

		// Calculate the average rating
		double averageRating = await opponentRatings.AverageAsync(x => x.Mu);

		return (int)averageRating;
	}

	public async Task<string?> BestPerformingTeammateNameAsync(long osuPlayerId, int mode, DateTime fromDate)
	{
		var verifiedScores = _context.MatchScores.WhereVerified();
		var teammateScores = verifiedScores.WhereTeammate(osuPlayerId)
		                                   .WhereMode(mode)
		                                   .After(fromDate);

		if (!teammateScores.Any())
		{
			return null;
		}

		var teammatePlayerIds = await teammateScores.Select(x => x.PlayerId).ToListAsync();
		
		teammatePlayerIds = teammatePlayerIds.Distinct().ToList();

		var teammateRatings = _context.Ratings
		                              .Where(x => teammatePlayerIds.Contains(x.PlayerId) && x.Mode == mode);

		double bestRating = await teammateRatings.MaxAsync(x => x.Mu);
		int bestRatingPlayerId = await teammateRatings.Where(x => x.Mu == bestRating).Select(x => x.PlayerId).FirstOrDefaultAsync();
		string? bestRatingPlayerName = await _context.Players.Where(x => x.Id == bestRatingPlayerId).Select(x => x.Username).FirstOrDefaultAsync();

		return bestRatingPlayerName;
	}

	public async Task<string?> WorstPerformingTeammateNameAsync(long osuPlayerId, int mode, DateTime fromDate)
	{
		var verifiedScores = _context.MatchScores.WhereVerified();
		var teammateScores = verifiedScores.WhereTeammate(osuPlayerId)
		                                   .WhereMode(mode)
		                                   .After(fromDate);
		
		if (!teammateScores.Any())
		{
			return null;
		}

		var teammatePlayerIds = await teammateScores
		                              .Select(x => x.PlayerId)
		                              .ToListAsync();

		teammatePlayerIds = teammatePlayerIds.Distinct().ToList();

		var teammateRatings = _context.Ratings
		                              .Where(x => teammatePlayerIds.Contains(x.PlayerId) && x.Mode == mode);

		double worstRating = await teammateRatings.MinAsync(x => x.Mu);
		int worstRatingPlayerId = await teammateRatings.Where(x => x.Mu == worstRating).Select(x => x.PlayerId).FirstOrDefaultAsync();
		string? worstRatingPlayerName = await _context.Players.Where(x => x.Id == worstRatingPlayerId).Select(x => x.Username).FirstOrDefaultAsync();

		return worstRatingPlayerName;
	}

	public async Task<string?> BestPerformingOpponentNameAsync(long osuPlayerId, int mode, DateTime fromDate)
	{
		var verifiedScores = _context.MatchScores.WhereVerified();
		var opponentScores = verifiedScores.WhereOpponent(osuPlayerId)
		                                   .WhereMode(mode)
		                                   .After(fromDate);
		
		if (!opponentScores.Any())
		{
			return null;
		}

		var opponentPlayerIds = await opponentScores
		                              .Select(x => x.PlayerId)
		                              .ToListAsync();

		opponentPlayerIds = opponentPlayerIds.Distinct().ToList();

		var opponentRatings = _context.Ratings
		                              .Where(x => opponentPlayerIds.Contains(x.PlayerId) && x.Mode == mode);

		double bestRating = await opponentRatings.MaxAsync(x => x.Mu);
		int bestRatingPlayerId = await opponentRatings.Where(x => x.Mu == bestRating).Select(x => x.PlayerId).FirstOrDefaultAsync();
		string? bestRatingPlayerName = await _context.Players.Where(x => x.Id == bestRatingPlayerId).Select(x => x.Username).FirstOrDefaultAsync();

		return bestRatingPlayerName;
	}

	public async Task<string?> WorstPerformingOpponentNameAsync(long osuPlayerId, int mode, DateTime fromDate)
	{
		var verifiedScores = _context.MatchScores.WhereVerified();
		var opponentScores = verifiedScores
		                     .WhereMode(mode)
		                     .WhereOpponent(osuPlayerId)
		                     .After(fromDate);
		
		if (!opponentScores.Any())
		{
			return null;
		}

		var opponentPlayerIds = await opponentScores
		                              .Select(x => x.PlayerId)
		                              .ToListAsync();

		opponentPlayerIds = opponentPlayerIds.Distinct().ToList();

		var opponentRatings = _context.Ratings
		                              .Where(x => opponentPlayerIds.Contains(x.PlayerId) && x.Mode == mode);

		double worstRating = await opponentRatings.MinAsync(x => x.Mu);
		int worstRatingPlayerId = await opponentRatings.Where(x => x.Mu == worstRating).Select(x => x.PlayerId).FirstOrDefaultAsync();
		string? worstRatingPlayerName = await _context.Players.Where(x => x.Id == worstRatingPlayerId).Select(x => x.Username).FirstOrDefaultAsync();

		return worstRatingPlayerName;
	}

	public async Task<DateTime> GetRecentCreatedDate(long osuPlayerId) =>
		await _context.Ratings.WherePlayer(osuPlayerId).OrderByDescending(x => x.Created).Select(x => x.Created).FirstAsync();

	public async Task<bool> IsRatingPositiveTrendAsync(long osuId, int modeInt, DateTime time)
	{
		double ratingPrevious = await _context.RatingHistories
		                                      .WherePlayer(osuId)
		                                      .WhereMode(modeInt)
		                                      .OrderByDescending(x => x.Created)
		                                      .Where(x => x.Created <= time)
		                                      .Select(x => x.Mu)
		                                      .FirstOrDefaultAsync();

		double ratingCurrent = await _context.Ratings.WherePlayer(osuId).WhereMode(modeInt).Select(x => x.Mu).FirstOrDefaultAsync();

		return ratingCurrent > ratingPrevious;
	}
}