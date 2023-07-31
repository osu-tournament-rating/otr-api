using API.Configurations;
using API.Entities;
using API.Services.Interfaces;

namespace API.Services.Implementations;

public class MatchDataService : ServiceBase<MatchData>, IMatchDataService
{
	public MatchDataService(IDbCredentials dbCredentials) : base(dbCredentials) {}
}