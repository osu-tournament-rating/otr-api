using API.Services.Implementations;
using API.Services.Interfaces;
using APITests.Framework.MockRepositories;
using Moq;

namespace APITests.Framework;

public static class MockServices
{
    public static IBaseStatsService BaseStatsService()
    {
        API.Repositories.Interfaces.IBaseStatsRepository baseStatsRepository = new MockBaseStatsRepository().Object;
        API.Repositories.Interfaces.IPlayerMatchStatsRepository playerMatchStatsRepository = new MockPlayerMatchStatsRepository().Object;
        API.Repositories.Interfaces.IMatchRatingStatsRepository matchRatingStatsRepository = new MockMatchRatingStatsRepository().Object;
        API.Repositories.Interfaces.IPlayerRepository playerRepository = new MockPlayerRepository().Object;
        var tournamentsService = new MockTournamentsService().Object;

        return new BaseStatsService()
    }

    public static ITournamentsService TournamentsService()
    {
        API.Repositories.Interfaces.ITournamentsRepository tournamentsRepository = new MockTournamentsRepository().Object;
        AutoMapper.IMapper mapper = MapperUtils.Instance();

        return new TournamentsService(tournamentsRepository, mapper);
    }
}
