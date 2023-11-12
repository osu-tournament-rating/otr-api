using API;
using API.Services.Implementations;
using API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace APITests.Instances;

public static class ServiceInstances
{
	public static IBaseStatsService BaseStatsService(OtrContext context) => new BaseStatsService(RepositoryInstances.BaseStatsRepository(context),
		RepositoryInstances.PlayerMatchStatsRepository(context),
		RepositoryInstances.MatchRatingStatsRepository(context),
		RepositoryInstances.PlayerRepository(context));

	public static IBeatmapService BeatmapService(OtrContext context) => new BeatmapService(RepositoryInstances.BeatmapRepository(context), ConfigurationInstances.Mapper);
	public static IPlayerService PlayerService(OtrContext context) => new PlayerService(RepositoryInstances.PlayerRepository(context), ConfigurationInstances.Mapper);

	public static IPlayerStatsService PlayerStatsService(OtrContext context) => new PlayerStatsService(RepositoryInstances.PlayerRepository(context),
		RepositoryInstances.PlayerMatchStatsRepository(context),
		RepositoryInstances.MatchRatingStatsRepository(context), PlayerScoreStatsService(context), RepositoryInstances.TournamentsRepository(context), BaseStatsService(context),
		ConfigurationInstances.Mapper);

	public static ILeaderboardService LeaderboardService(OtrContext context) => new LeaderboardService(RepositoryInstances.PlayerRepository(context),
		BaseStatsService(context), RepositoryInstances.MatchRatingStatsRepository(context), PlayerService(context), PlayerStatsService(context));

	public static IMatchesService MatchesService(OtrContext context) => new MatchesService(Mock.Of<ILogger<MatchesService>>(), RepositoryInstances.MatchesRepository(context),
		RepositoryInstances.TournamentsRepository(context), ConfigurationInstances.Mapper);

	public static IPlayerScoreStatsService PlayerScoreStatsService(OtrContext context) => new PlayerScoreStatsService(RepositoryInstances.MatchScoresRepository(context));

	public static ITournamentsService TournamentsService(OtrContext context) =>
		new TournamentsService(RepositoryInstances.TournamentsRepository(context), ConfigurationInstances.Mapper);

	public static IUserService UserService(OtrContext context) => new UserService(RepositoryInstances.UserRepository(context));
}