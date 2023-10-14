using API.Osu;
using API.Services.Implementations;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;

namespace APITests.Services;

[Collection("DatabaseCollection")]
public class MatchesServiceTests
{
	private readonly Mock<ILogger<MatchesService>> _matchesServiceMock = new();
	private readonly Mock<IMapper> _mapper = new();
	private readonly Mock<IGameSrCalculator> _gameSrCalculator = new();
	
	public MatchesServiceTests(TestDatabaseFixture testDatabaseFixture) { _fixture = testDatabaseFixture; }

	private readonly TestDatabaseFixture _fixture;

	[Fact]
	public async Task GetMatch_ByOsuId_Returns_Correct_Values()
	{
		// arrange
		using var context = _fixture.Context;
		var matchesService = new MatchesService(_matchesServiceMock.Object, _mapper.Object, context, _gameSrCalculator.Object);

		// act
		var match = await matchesService.GetByMatchIdAsync(105297522);

		// assert
		Assert.NotNull(match);
		Assert.Equal(105297522, match.MatchId);
		
		Assert.Equal(8, match.Games.Count);

		foreach (var game in match.Games)
		{
			Assert.Equal(8, game.MatchScores.Count);
		}
	}
}