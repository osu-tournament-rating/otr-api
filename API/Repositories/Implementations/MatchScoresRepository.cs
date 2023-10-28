using API.Entities;
using API.Osu;
using API.Repositories.Interfaces;
using API.Utilities;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class MatchScoresRepository : RepositoryBase<MatchScore>, IMatchScoresRepository
{
	private readonly OtrContext _context;
	public MatchScoresRepository(OtrContext context) : base(context) { _context = context; }

	public async Task<int> AverageTeammateScoreAsync(long osuPlayerId, int mode, DateTime fromTime)
	{
		var teammateScores = await _context.MatchScores
		                                        .WhereVerified()
		                                        .After(fromTime)
		                                        .WhereMode(mode)
		                                        .WhereTeammate(osuPlayerId)
		                                        .Select(ms => ms.Score)
		                                        .ToListAsync();
		return (int) teammateScores.Average();
	}

	public async Task<int> AverageOpponentScoreAsync(long osuPlayerId, int mode, DateTime fromTime)
	{
		var oppScoresHeadToHead = await _context.MatchScores
		                                        .WhereVerified()
		                                        .After(fromTime)
		                                        .WhereMode(mode)
		                                        .WhereHeadToHead()
		                                        .WhereOpponent(osuPlayerId)
		                                        .Select(ms => ms.Score)
		                                        .ToListAsync();
		
		var oppScoresTeamVs = await _context.MatchScores
		                                    .WhereVerified()
		                                    .After(fromTime)
		                                    .WhereMode(mode)
		                                    .WhereTeamVs()
		                                    .WhereOpponent(osuPlayerId)
		                                    .Select(ms => ms.Score)
		                                    .ToListAsync();
		
		var oppScores = oppScoresHeadToHead.Concat(oppScoresTeamVs);
		return (int) oppScores.Average();
	}

	public async Task<int> AverageModScoreAsync(int playerId, int mode, int mods, DateTime dateMin,
		DateTime dateMax)
	{
		try
		{
			return (int) await _context.MatchScores
			                          .WhereVerified()
			                          .WhereMode(mode)
			                          .WhereMods((OsuEnums.Mods)mods)
			                          .WherePlayerId(playerId)
			                          .WhereDateRange(dateMin, dateMax)
			                          .Select(x => x.Score)
			                          .AverageAsync();
		}
		catch (InvalidOperationException)
		{
			// Thrown when no scores are found
			return default;
		}
	}

	public async Task<int> CountModScoresAsync(int playerId, int mode, int mods, DateTime dateMin,
		DateTime dateMax) => await _context.MatchScores
		                                   .WhereVerified()
		                                   .WhereMode(mode)
		                                   .WhereMods((OsuEnums.Mods)mods)
		                                   .WherePlayerId(playerId)
		                                   .WhereDateRange(dateMin, dateMax)
		                                   .CountAsync();
}