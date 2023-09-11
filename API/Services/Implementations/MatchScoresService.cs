using API.Entities;
using API.Services.Interfaces;

namespace API.Services.Implementations;

public class MatchScoresService : ServiceBase<MatchScore>, IMatchScoresService
{
	public MatchScoresService(ILogger<MatchScoresService> logger, OtrContext context) : base(logger, context) {}
}