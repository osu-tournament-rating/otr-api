using API.Services.Implementations;
using API.Services.Interfaces;
using APITests.Framework.MockRepositories;

namespace APITests.Framework;

public static class MockServices
{
    public static IBaseStatsService BaseStatsService()
    {
        API.Repositories.Interfaces.IBaseStatsRepository baseStatsRepository = new MockBaseStatsRepository().Object;
        API.Repositories.Interfaces.IPlayerMatchStatsRepository playerMatchStatsRepository = new MockPlayerMatchStatsRepository().Object;
        API.Repositories.Interfaces.IMatchRatingStatsRepository matchRatingStatsRepository = new MockMatchRatingStatsRepository().Object;
        API.Repositories.Interfaces.IPlayerRepository playerRepository = new MockPlayerRepository().Object;
        ITournamentsService tournamentsService = TournamentsService();

        return new BaseStatsService(baseStatsRepository, playerMatchStatsRepository, matchRatingStatsRepository,
            playerRepository, tournamentsService);
    }

    public static IBeatmapService BeatmapService()
    {
        API.Repositories.Interfaces.IBeatmapRepository beatmapRepository = new MockBeatmapRepository().Object;
        AutoMapper.IMapper mapper = MapperUtils.Instance();

        return new BeatmapService(beatmapRepository, mapper);
    }

    public static ILeaderboardService LeaderboardService()
    {
        API.Repositories.Interfaces.IPlayerRepository playerRepository = new MockPlayerRepository().Object;
        API.Repositories.Interfaces.IMatchRatingStatsRepository matchRatingStatsRepository = new MockMatchRatingStatsRepository().Object;

        IBaseStatsService baseStatsService = BaseStatsService();
        PlayerService playerService = PlayerService();
        IPlayerStatsService playerStatsService = PlayerStatsService();

        return new LeaderboardService(playerRepository, baseStatsService,
            matchRatingStatsRepository, playerService, playerStatsService);
    }

    public static PlayerService PlayerService()
    {
        API.Repositories.Interfaces.IPlayerRepository playerRepository = new MockPlayerRepository().Object;
        AutoMapper.IMapper mapper = MapperUtils.Instance();

        return new PlayerService(playerRepository, mapper);
    }

    public static IPlayerStatsService PlayerStatsService()
    {
        AutoMapper.IMapper mapper = MapperUtils.Instance();

        API.Repositories.Interfaces.IGameWinRecordsRepository gameWinRecordsRepository = new MockGameWinRecordsRepository().Object;
        API.Repositories.Interfaces.IMatchRatingStatsRepository matchRatingStatsRepository = new MockMatchRatingStatsRepository().Object;
        API.Repositories.Interfaces.IMatchWinRecordsRepository matchWinRecordsRepository = new MockMatchWinRecordsRepository().Object;
        API.Repositories.Interfaces.IPlayerMatchStatsRepository playerMatchStatsRepository = new MockPlayerMatchStatsRepository().Object;
        API.Repositories.Interfaces.IPlayerRepository playerRepository = new MockPlayerRepository().Object;
        API.Repositories.Interfaces.IRatingAdjustmentsRepository ratingAdjustmentsRepository = new MockRatingAdjustmentsRepository().Object;
        API.Repositories.Interfaces.ITournamentsRepository tournamentsRepository = new MockTournamentsRepository().Object;

        IBaseStatsService baseStatsService = BaseStatsService();
        var playerService = new PlayerService(playerRepository, mapper);

        return new PlayerStatsService(baseStatsService, gameWinRecordsRepository, matchWinRecordsRepository,
            playerMatchStatsRepository, playerService, playerRepository, ratingAdjustmentsRepository,
            matchRatingStatsRepository, tournamentsRepository);
    }

    public static ITournamentsService TournamentsService()
    {
        API.Repositories.Interfaces.ITournamentsRepository tournamentsRepository = new MockTournamentsRepository().Object;
        AutoMapper.IMapper mapper = MapperUtils.Instance();

        return new TournamentsService(tournamentsRepository, mapper);
    }
}
