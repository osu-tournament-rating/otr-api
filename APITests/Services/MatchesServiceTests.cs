using API.Repositories.Implementations;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;

namespace APITests.Services;

[Collection("DatabaseCollection")]
public class MatchesServiceTests
{
	private readonly Mock<ILogger<MatchesRepository>> _matchesServiceMock = new();
	private readonly Mock<IMapper> _mapper = new();
	
	public MatchesServiceTests(TestDatabaseFixture testDatabaseFixture) { _fixture = testDatabaseFixture; }

	private readonly TestDatabaseFixture _fixture;

	[Fact]
	public async Task GetMatch_ByOsuId_Returns_Correct_Values()
	{
		// arrange
		using var context = _fixture.CreateContext();
		var matchesService = new MatchesRepository(_matchesServiceMock.Object, _mapper.Object, context);

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
		
		// assert - details
		Assert.NotNull(match.Tournament);
		
		Assert.Equal("osu! World Cup 2022", match.Tournament.Name);
		Assert.Equal("OWC2022", match.Tournament.Abbreviation);
		Assert.Equal("https://osu.ppy.sh/wiki/en/Tournaments/OWC/2022", match.Tournament.ForumUrl);
		Assert.Equal(0, match.Tournament.Mode);
		Assert.Equal(4, match.Tournament.TeamSize);
	}
}