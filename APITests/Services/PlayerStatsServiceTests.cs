using API;
using API.Repositories.Implementations;
using API.Services.Implementations;
using API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace APITests.Services;

// [Collection("DatabaseCollection")]
// public class PlayerStatsServiceTests
// {
// 	private readonly TestDatabaseFixture _fixture;
//
// 	public PlayerStatsServiceTests(TestDatabaseFixture fixture) { _fixture = fixture; }
//
// 	[Fact]
// 	public async Task PlayerStatistics_Schema_IsValid()
// 	{
// 		// arrange
// 		using var context = _fixture.CreateContext();
// 		var playerStatsService = GetMatchStatsService(context);
// 		
// 		// act
// 		// Get a dummy player to test against
// 	
// 		// assert
// 	}
// 	
// 	// TODO: DTO that aggregates a collection of stats, including 
//
// 	private IPlayerStatsService GetMatchStatsService(OtrContext context)
// 	{
// 		return new PlayerStatsService();
// 	}
// }