using API;
using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace APITests.Services;

[Collection("DatabaseCollection")]
public class PlayerStatisticsServiceTests
{
	private readonly TestDatabaseFixture _fixture;

	public PlayerStatisticsServiceTests(TestDatabaseFixture fixture) { _fixture = fixture; }

	[Fact]
	public async Task PlayerStatistics_Schema_IsValid()
	{
		// arrange
		using var context = _fixture.CreateContext();
		var playerStatsService = GetGameStatsRepository(context);
		var matchStatsService = GetMatchStatsRepository(context);
		
		// act
		// Get a dummy player to test against
		var player = await context.Players.FirstOrDefaultAsync();
		var dateMin = DateTime.MinValue;
		var dateMax = DateTime.MaxValue;
		const int mode = 0;
		var gameStats = (await playerStatsService.GetForPlayerAsync(player!.Id, mode, dateMin, dateMax)).First();
		var matchStats = (await matchStatsService.GetForPlayerAsync(player.Id, mode, dateMin, dateMax)).First();

		// assert
		Assert.NotNull(gameStats);
		
		Assert.IsType<int>(gameStats.Id);
		Assert.IsType<int>(gameStats.PlayerId);
		Assert.IsType<int>(gameStats.GameId);
		Assert.IsType<int>(gameStats.Mode);
		
		Assert.IsType<bool>(gameStats.Won);
		Assert.IsType<double>(gameStats.AverageOpponentRating);
		Assert.IsType<double>(gameStats.AverageTeammateRating);
		Assert.IsType<int>(gameStats.Placing);
		Assert.IsType<bool>(gameStats.PlayedHR);
		Assert.IsType<bool>(gameStats.PlayedHD);
		Assert.IsType<bool>(gameStats.PlayedDT);

		Assert.IsType<int[]>(gameStats.TeammateIds);
		Assert.IsType<int[]>(gameStats.OpponentIds);

		Assert.IsType<int>(matchStats.PlayerId);
		Assert.IsType<int>(matchStats.MatchId);
		Assert.IsType<bool>(matchStats.Won);
		Assert.IsType<double>(matchStats.MatchCost);
		Assert.IsType<int>(matchStats.PointsEarned);
		Assert.IsType<double>(matchStats.RatingBefore);
		Assert.IsType<double>(matchStats.RatingAfter);
		Assert.IsType<double>(matchStats.RatingChange);
		Assert.IsType<double>(matchStats.VolatilityBefore);
		Assert.IsType<double>(matchStats.VolatilityAfter);
		Assert.IsType<int>(matchStats.AverageScore);
		Assert.IsType<int>(matchStats.AverageMisses);
		Assert.IsType<double>(matchStats.AverageAccuracy);
		Assert.IsType<int>(matchStats.GamesPlayed);
		
		Assert.IsType<int>(matchStats.GlobalRankBefore);
		Assert.IsType<int>(matchStats.GlobalRankAfter);
		Assert.IsType<int>(matchStats.CountryRankBefore);
		Assert.IsType<int>(matchStats.CountryRankAfter);
		Assert.IsType<double>(matchStats.PercentileBefore);
		Assert.IsType<double>(matchStats.PercentileAfter);
	}
	
	// TODO: DTO that aggregates a collection of stats, including 

	private IPlayerGameStatisticsRepository GetGameStatsRepository(OtrContext context)
	{
		var loggerMock = new Mock<ILogger<PlayerGameStatisticsRepository>>();
		return new PlayerGameStatisticsRepository(context);
	}
	
	private IPlayerMatchStatisticsRepository GetMatchStatsRepository(OtrContext context)
	{
		var loggerMock = new Mock<ILogger<PlayerMatchStatisticsRepository>>();
		return new PlayerMatchStatisticsRepository(context);
	}
}