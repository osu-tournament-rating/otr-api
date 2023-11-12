using API.DTOs;
using APITests.Instances;
using Microsoft.AspNetCore.Mvc;

namespace APITests.Controllers;

[Collection("DatabaseCollection")]
public class LeaderboardControllerTests
{
	private readonly TestDatabaseFixture _fixture;

	public LeaderboardControllerTests(TestDatabaseFixture fixture) { _fixture = fixture; }

	[Fact]
	public async Task Leaderboard_ReturnsAllStandard_WhenModeStandard()
	{
		using var context = _fixture.CreateContext();
		var controller = ControllerInstances.LeaderboardsController(context);

		var query = new LeaderboardRequestQueryDTO
		{
			Mode = 0
		};

		var actionResult = await controller.GetAsync(query);

		Assert.IsType<OkObjectResult>(actionResult.Result);

		var okResult = actionResult.Result as OkObjectResult;
		Assert.IsType<LeaderboardDTO>(okResult!.Value);

		var leaderboard = okResult.Value as LeaderboardDTO;
		
		Assert.All(leaderboard!.PlayerInfo, pInfo => Assert.Equal(0, pInfo.Mode));
	}
}