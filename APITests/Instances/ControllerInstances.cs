using API;
using API.Controllers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace APITests.Instances;

public static class ControllerInstances
{
	public static LeaderboardsController LeaderboardsController(OtrContext context) => new(ServiceInstances.LeaderboardService(context), Mock.Of<IConfiguration>());

	public static MatchesController MatchesController(OtrContext context) => new(Mock.Of<ILogger<MatchesController>>(), ServiceInstances.MatchesService(context),
		ServiceInstances.TournamentsService(context));

	public static StatsController StatsController(OtrContext context) =>
		new(Mock.Of<ILogger<StatsController>>(), Mock.Of<IDistributedCache>(), ServiceInstances.PlayerStatsService(context));
}