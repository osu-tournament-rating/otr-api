using API;
using API.Controllers;
using API.DTOs;
using API.Entities;
using API.Repositories.Implementations;
using API.Services.Implementations;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;

namespace APITests.Controllers;

[Collection("DatabaseCollection")]
public class StatsControllerTests
{
	private readonly TestDatabaseFixture _fixture;
	public StatsControllerTests(TestDatabaseFixture fixture) { _fixture = fixture; }

	[Fact]
	public async Task StatsController_ReturnsStats()
	{
		// arrange
		using var context = _fixture.CreateContext();
		var controller = GetController(context);

		// act
		var result = await controller.GetAsync(8191845, 0, DateTime.MinValue, DateTime.MaxValue);

		// assert
		Assert.IsType<OkObjectResult>(result.Result);
		
		var okResult = result.Result as OkObjectResult;
		Assert.IsType<PlayerStatisticsDTO>(okResult?.Value);
		
		var stats = okResult?.Value as PlayerStatisticsDTO;
		var matchStats = stats?.MatchStatistics;
		var scoreStats = stats?.ScoreStatistics;

		// Assert.NotNull(matchStats);
		Assert.NotNull(scoreStats);

		Assert.IsType<int>(scoreStats.AverageScoreNM);
		Assert.IsType<int>(scoreStats.AverageScoreEZ);
		Assert.IsType<int>(scoreStats.AverageScoreHD);
		Assert.IsType<int>(scoreStats.AverageScoreHR);
		Assert.IsType<int>(scoreStats.AverageScoreFL);
		Assert.IsType<int>(scoreStats.AverageScoreHDDT);
		Assert.IsType<int>(scoreStats.AverageScoreHDHR);
		
		Assert.IsType<int>(scoreStats.CountPlayedNM);
		Assert.IsType<int>(scoreStats.CountPlayedEZ);
		Assert.IsType<int>(scoreStats.CountPlayedHD);
		Assert.IsType<int>(scoreStats.CountPlayedHR);
		Assert.IsType<int>(scoreStats.CountPlayedFL);
		Assert.IsType<int>(scoreStats.CountPlayedHDDT);
		Assert.IsType<int>(scoreStats.CountPlayedHDHR);
	}

	[Fact]
	public async Task StatsPost_Inserts_Data()
	{
		// arrange
		using var context = _fixture.CreateContext();
		var controller = GetController(context);

		// act
		var postBody = new PlayerMatchStatistics
		{
			PlayerId = 440,
			MatchId = 8880,
			RatingBefore = 1000D,
			RatingAfter = 1000D,
			GlobalRankBefore = 1000,
			GlobalRankAfter = 1000,
			CountryRankBefore = 1000,
			CountryRankAfter = 1000,
			PercentileBefore = 1000D,
			PercentileAfter = 0.95,
			Won = true,
			GamesWon = 1,
			GamesLost = 1,
			AverageTeammateRating = 414.5,
			AverageOpponentRating = 821.291,
			AverageScore = 1000,
			AverageAccuracy = 1000,
			AverageMisses = 1000,
			GamesPlayed = 3,
			AveragePlacement = 1D,
			TeammateIds = new int[] { 1, 2, 3 },
			OpponentIds = new int[] { 1, 2, 3 }
		};

		context.Database.BeginTransaction();
		var result = await controller.PostAsync(postBody);

		Assert.IsType<OkResult>(result);
		
		context.ChangeTracker.Clear();

		// assert
		var data = await context.PlayerMatchStatistics
		                          .Where(x => x.PlayerId == postBody.PlayerId)
		                          .Where(x => x.MatchId == postBody.MatchId)
		                          .Where(x => x.RatingBefore == postBody.RatingBefore)
		                          .Where(x => x.RatingAfter == postBody.RatingAfter)
		                          .Where(x => x.GlobalRankAfter == postBody.GlobalRankAfter)
		                          .FirstOrDefaultAsync();

		Assert.NotNull(data);

		Assert.Equal(postBody.PercentileBefore, data.PercentileBefore);
		Assert.Equal(postBody.PercentileAfter, data.PercentileAfter);
	}

	private StatsController GetController(OtrContext context)
	{
		var loggerMock = Mock.Of<ILogger<StatsController>>();
		var cacheMock = Mock.Of<IDistributedCache>();
		var serviceProviderMock = Mock.Of<IServiceProvider>();
		var mapperMock = Mock.Of<IMapper>();

		var playerRepository = new PlayerRepository(context, serviceProviderMock, mapperMock);
		var playerMatchStatsRepository = new PlayerMatchStatisticsRepository(context);
		var matchScoresRepository = new MatchScoresRepository(context);
		var playerScoreStatsService = new PlayerScoreStatisticsService(matchScoresRepository);
		var statsService = new PlayerStatisticsService(playerRepository, playerMatchStatsRepository, playerScoreStatsService);
		return new StatsController(loggerMock, cacheMock, statsService);
	}
}