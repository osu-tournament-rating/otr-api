using API;
using API.Controllers;
using API.Repositories.Implementations;
using API.Services.Implementations;
using APITests.Instances;
using AutoMapper;
using Moq;

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
	}
}