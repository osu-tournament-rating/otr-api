using API;
using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using API.Services.Implementations;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace APITests.Repositories;

[Collection("DatabaseCollection")]
public class PlayerStatisticsServiceTests
{
	private readonly TestDatabaseFixture _fixture;

	public PlayerStatisticsServiceTests(TestDatabaseFixture fixture) { _fixture = fixture; }

	// [Fact]
	// public async Task PlayerStatistics_Schema_IsValid()
	// {
	// 	// arrange
	// 	using var context = _fixture.CreateContext();
	// 	var playerStatsService = GetMatchStatsService(context);
	// 	
	// 	// act
	// 	// Get a dummy player to test against
	//
	// 	// assert
	// }
	
	// TODO: DTO that aggregates a collection of stats, including 

	// private IPlayerStatisticsService GetMatchStatsService(OtrContext context)
	// {
	// 	var loggerMock = new Mock<ILogger<PlayerMatchStatisticsRepository>>();
	// 	return new PlayerStatisticsService();
	// }
}