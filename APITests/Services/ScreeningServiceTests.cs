using API.Configurations;
using API.Entities;
using API.Services.Implementations;
using APITests.MockRepositories;
using AutoMapper;

namespace APITests.Services;

public class ScreeningServiceTests
{
    public ScreeningServiceTests()
    {
        MockPlayerRepository playerRepository = new MockPlayerRepository()
            .SetupGet()
            .SetupGetId();

        MockBaseStatsRepository baseStatsRepository = new MockBaseStatsRepository()
            .SetupHighestRating()
            .SetupGetForPlayerAsync();

        var mapper = new MapperConfiguration(x => { x.AddProfile<MapperProfile>(); });

        var playerService = new PlayerService(playerRepository.Object, mapper.CreateMapper());
        var baseStatsService = new BaseStatsService()

        var screeningService = new ScreeningService(playerService)
    }
}
