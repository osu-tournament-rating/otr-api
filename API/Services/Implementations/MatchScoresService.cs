using API.Entities;
using API.Services.Interfaces;
using API.Utilities;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class MatchScoresService : ServiceBase<MatchScore>, IMatchScoresService
{
	private readonly OtrContext _context;
	public MatchScoresService(ILogger<MatchScoresService> logger, OtrContext context) : base(logger, context) { _context = context; }

	public async Task<int> AverageTeammateScore(long osuPlayerId, int mode, DateTime fromTime)
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

	public async Task<int> AverageOpponentScore(long osuPlayerId, int mode, DateTime fromTime)
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
}