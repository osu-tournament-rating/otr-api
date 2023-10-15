using API;
using API.Repositories.Implementations;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;

namespace APITests.Services;

[Collection("DatabaseCollection")]
public class ApiMatchServiceTests
{
	private readonly TestDatabaseFixture _fixture;

	public ApiMatchServiceTests(TestDatabaseFixture fixture)
	{
		_fixture = fixture;
	}
	
	private ApiMatchRepository ApiMatchService(OtrContext context)
	{
		var loggerMock = new Mock<ILogger<ApiMatchRepository>>();
		var playerLoggerMock = new Mock<ILogger<PlayerRepository>>();
		var beatmapLoggerMock = new Mock<ILogger<BeatmapRepository>>();
		var matchLoggerMock = new Mock<ILogger<MatchesRepository>>();
		var gamesLoggerMock = new Mock<ILogger<GamesRepository>>();
		var matchScoresLoggerMock = new Mock<ILogger<MatchScoresRepository>>();
		var mapperMock = new Mock<IMapper>();


		return null; // Come back...
	}
}