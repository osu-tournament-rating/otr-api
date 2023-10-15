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
		var playerStatsService = GetService(context);
		var matchStatsService = GetMatchStatsService(context);
		
		// act
		// Get a dummy player to test against
		var player = await context.Players.FirstOrDefaultAsync();
		var dateMin = DateTime.MinValue;
		var dateMax = DateTime.MaxValue;
		const int mode = 0;
		var stats = await playerStatsService.GetForPlayerAsync(player.Id, mode, dateMin, dateMax);
		var matchStats = await matchStatsService.GetForPlayerAsync(player.Id, mode, dateMin, dateMax);

		// assert
		Assert.NotNull(stats);
		
		Assert.IsType<int>(stats.Id);
		Assert.IsType<string>(stats.Tier);
		Assert.IsType<int>(stats.Mode);
		Assert.IsType<int>(stats.CurrentRating);
		Assert.IsType<int>(stats.CurrentGlobalRank);
		Assert.IsType<int>(stats.CurrentCountryRank);
		Assert.IsType<double>(stats.Percentile);
		Assert.IsType<int>(stats.RatingLeftForNextTier);
		Assert.IsType<int>(stats.RatingGained);
		Assert.IsType<int>(stats.HighestRating);
		Assert.IsType<int>(stats.MatchesPlayed);
		Assert.IsType<int>(stats.MapsPlayed);
		Assert.IsType<int>(stats.MatchesWon);
		Assert.IsType<int>(stats.MapsWon);
		Assert.IsType<int>(stats.AverageOpponentRating);
		Assert.IsType<int>(stats.AverageTeammateRating);
		Assert.IsType<int>(stats.BestWinStreak);
		Assert.IsType<int>(stats.AverageScore);
		Assert.IsType<int>(stats.AverageMisses);
		Assert.IsType<int>(stats.AverageAccuracy);
		Assert.IsType<int>(stats.AveragePlacing);
		Assert.IsType<int>(stats.PlayedHR);
		Assert.IsType<int>(stats.PlayedHD);
		Assert.IsType<int>(stats.PlayedDT);

		Assert.IsType<string>(stats.MostPlayedTeammate);
		Assert.IsType<string>(stats.MostPlayedOpponent);
		Assert.IsType<string>(stats.BestTeammate);
		Assert.IsType<string>(stats.BestOpponent);
		Assert.IsType<string>(stats.WorstTeammate);
		Assert.IsType<string>(stats.WorstOpponent);

		Assert.IsType<int>(matchStats.Rating);
		Assert.IsType<int>()
		Assert.IsType<int>(matchStats.AverageScore);
		Assert.IsType<int>(matchStats.AverageMisses);
		Assert.IsType<double>(matchStats.AverageAccuracy);
		Assert.IsType<int>(matchStats.AverageMapsPlayed);
	}

	private IPlayerStatisticsRepository GetService(OtrContext context)
	{
		var loggerMock = new Mock<ILogger<PlayerStatisticsRepository>>();
		return new PlayerStatisticsRepository(loggerMock.Object, context);
	}
	
	private IPlayerMatchStatisticsService GetMatchStatsService(OtrContext context)
	{
		var loggerMock = new Mock<ILogger<PlayerMatchStatisticsService>>();
		return new PlayerMatchStatisticsService(loggerMock.Object, context);
	}
}