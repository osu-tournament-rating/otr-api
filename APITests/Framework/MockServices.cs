using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using API.Services.Implementations;
using API.Services.Interfaces;
using APITests.Framework.MockRepositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace APITests.Framework;

public static class MockServices
{
    public static IBaseStatsService BaseStatsService()
    {
        IBaseStatsRepository baseStatsRepository = new MockBaseStatsRepository().Object;
        IPlayerMatchStatsRepository playerMatchStatsRepository = new MockPlayerMatchStatsRepository().Object;
        IMatchRatingStatsRepository matchRatingStatsRepository = new MockMatchRatingStatsRepository().Object;
        IPlayerRepository playerRepository = new MockPlayerRepository().Object;
        ITournamentsService tournamentsService = TournamentsService();

        return new BaseStatsService(baseStatsRepository, playerMatchStatsRepository, matchRatingStatsRepository,
            playerRepository, tournamentsService);
    }

    public static IBeatmapService BeatmapService()
    {
        IBeatmapRepository beatmapRepository = new MockBeatmapRepository().Object;
        AutoMapper.IMapper mapper = MapperUtils.Instance();

        return new BeatmapService(beatmapRepository, mapper);
    }

    public static ILeaderboardService LeaderboardService()
    {
        IPlayerRepository playerRepository = new MockPlayerRepository().Object;
        IMatchRatingStatsRepository matchRatingStatsRepository = new MockMatchRatingStatsRepository().Object;

        IBaseStatsService baseStatsService = BaseStatsService();
        PlayerService playerService = PlayerService();
        IPlayerStatsService playerStatsService = PlayerStatsService();

        return new LeaderboardService(playerRepository, baseStatsService,
            matchRatingStatsRepository, playerService, playerStatsService);
    }

    public static IMatchesService MatchesService()
    {
        ILogger<MatchesService> logger = new Mock<ILogger<MatchesService>>().Object;
        IMatchesRepository matchesRepository = new MockMatchesRepository().Object;
        ITournamentsRepository tournamentsRepository = new MockTournamentsRepository().Object;
        IMatchDuplicateRepository matchDuplicateRepository = new MockMatchDuplicateRepository().Object;
        AutoMapper.IMapper mapper = MapperUtils.Instance();

        return new MatchesService(logger, matchesRepository, tournamentsRepository,
            matchDuplicateRepository, mapper);
    }

    public static PlayerService PlayerService()
    {
        IPlayerRepository playerRepository = new MockPlayerRepository().Object;
        AutoMapper.IMapper mapper = MapperUtils.Instance();

        return new PlayerService(playerRepository, mapper);
    }

    public static IPlayerStatsService PlayerStatsService()
    {
        AutoMapper.IMapper mapper = MapperUtils.Instance();

        IGameWinRecordsRepository gameWinRecordsRepository = new MockGameWinRecordsRepository().Object;
        IMatchRatingStatsRepository matchRatingStatsRepository = new MockMatchRatingStatsRepository().Object;
        IMatchWinRecordsRepository matchWinRecordsRepository = new MockMatchWinRecordsRepository().Object;
        IPlayerMatchStatsRepository playerMatchStatsRepository = new MockPlayerMatchStatsRepository().Object;
        IPlayerRepository playerRepository = new MockPlayerRepository().Object;
        IRatingAdjustmentsRepository ratingAdjustmentsRepository = new MockRatingAdjustmentsRepository().Object;
        ITournamentsRepository tournamentsRepository = new MockTournamentsRepository().Object;

        IBaseStatsService baseStatsService = BaseStatsService();
        var playerService = new PlayerService(playerRepository, mapper);

        return new PlayerStatsService(baseStatsService, gameWinRecordsRepository, matchWinRecordsRepository,
            playerMatchStatsRepository, playerService, playerRepository, ratingAdjustmentsRepository,
            matchRatingStatsRepository, tournamentsRepository);
    }

    public static ITournamentsService TournamentsService()
    {
        ITournamentsRepository tournamentsRepository = new MockTournamentsRepository().Object;
        AutoMapper.IMapper mapper = MapperUtils.Instance();

        return new TournamentsService(tournamentsRepository, mapper);
    }
}
