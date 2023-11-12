using API;
using API.Osu.Multiplayer;
using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace APITests.Instances;

public static class RepositoryInstances
{
	public static IApiMatchRepository ApiMatchRepository(OtrContext context)
	{
		return new ApiMatchRepository(Mock.Of<ILogger<ApiMatchRepository>>(), context, PlayerRepository(context),
			BeatmapRepository(context), Mock.Of<IOsuApiService>(), MatchesRepository(context), GamesRepository(context),
			MatchScoresRepository(context));
	}
	
	public static IBaseStatsRepository BaseStatsRepository(OtrContext context)
	{
		return new BaseStatsRepository(context, PlayerRepository(context), PlayerMatchStatsRepository(context));
	}
	
	public static IMatchesRepository MatchesRepository(OtrContext context)
	{
		return new MatchesRepository(Mock.Of<ILogger<MatchesRepository>>(), ConfigurationInstances.Mapper, context);
	}
	
	public static ITournamentsRepository TournamentsRepository(OtrContext context)
	{
		return new TournamentsRepository(Mock.Of<ILogger<TournamentsRepository>>(), context, MatchesRepository(context));
	}
	
	public static IPlayerRepository PlayerRepository(OtrContext context)
	{
		return new PlayerRepository(context, ConfigurationInstances.Mapper);
	}
	
	public static IBeatmapRepository BeatmapRepository(OtrContext context)
	{
		return new BeatmapRepository(Mock.Of<ILogger<BeatmapRepository>>(), context);
	}

	public static IGamesRepository GamesRepository(OtrContext context)
	{
		return new GamesRepository(context);
	}
	
	public static IMatchScoresRepository MatchScoresRepository(OtrContext context)
	{
		return new MatchScoresRepository(context);
	}
	
	public static IMatchRatingStatsRepository MatchRatingStatsRepository(OtrContext context)
	{
		return new MatchRatingStatsRepository(context);
	}
	
	public static IPlayerMatchStatsRepository PlayerMatchStatsRepository(OtrContext context)
	{
		return new PlayerMatchStatsRepository(context);
	}
	
	public static IUserRepository UserRepository(OtrContext context)
	{
		return new UserRepository(Mock.Of<ILogger<UserRepository>>(), context);
	}
}