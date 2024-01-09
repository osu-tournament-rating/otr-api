using API;
using API.Osu.Multiplayer;
using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace APITests.Instances;

public static class RepositoryInstances
{
	public static IApiMatchRepository ApiMatchRepository(OtrContext context) => new ApiMatchRepository(Mock.Of<ILogger<ApiMatchRepository>>(), context, PlayerRepository(context),
		BeatmapRepository(context), Mock.Of<IOsuApiService>(), MatchesRepository(context), GamesRepository(context),
		MatchScoresRepository(context));

	public static IBaseStatsRepository BaseStatsRepository(OtrContext context) => new BaseStatsRepository(context, PlayerRepository(context));
	public static IMatchesRepository MatchesRepository(OtrContext context) => new MatchesRepository(Mock.Of<ILogger<MatchesRepository>>(), ConfigurationInstances.Mapper, context);

	public static ITournamentsRepository TournamentsRepository(OtrContext context) =>
		new TournamentsRepository(Mock.Of<ILogger<TournamentsRepository>>(), context, MatchesRepository(context));

	public static IPlayerRepository PlayerRepository(OtrContext context) => new PlayerRepository(context, ConfigurationInstances.Mapper);
	public static IBeatmapRepository BeatmapRepository(OtrContext context) => new BeatmapRepository(Mock.Of<ILogger<BeatmapRepository>>(), context);
	public static IGamesRepository GamesRepository(OtrContext context) => new GamesRepository(context);
	public static IMatchScoresRepository MatchScoresRepository(OtrContext context) => new MatchScoresRepository(context);
	public static IMatchRatingStatsRepository MatchRatingStatsRepository(OtrContext context) => new MatchRatingStatsRepository(context);
	public static IPlayerMatchStatsRepository PlayerMatchStatsRepository(OtrContext context) => new PlayerMatchStatsRepository(context);
	public static IUserRepository UserRepository(OtrContext context) => new UserRepository(Mock.Of<ILogger<UserRepository>>(), context);
	public static IRatingAdjustmentsRepository RatingAdjustmentsRepository(OtrContext context) => new RatingAdjustmentsRepository(context);
}