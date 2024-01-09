using API.Utilities;
using APITests.Instances;
using Microsoft.EntityFrameworkCore;

namespace APITests.Repositories;

[Collection("DatabaseCollection")]
public class BaseStatsRepositoryTests
{
	private readonly TestDatabaseFixture _fixture;
	public BaseStatsRepositoryTests(TestDatabaseFixture fixture) { _fixture = fixture; }

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(2)]
	[InlineData(3)]
	public async Task HighestRankAsync_ReturnsHighestRank(int mode)
	{
		// Arrange
		using var context = _fixture.CreateContext();
		var repository = RepositoryInstances.BaseStatsRepository(context);

		// Act
		int expectedHighestRank = await repository.HighestRankAsync(mode);
		int actualHighestRank = await context.BaseStats
		                                     .WhereMode(mode)
		                                     .MaxAsync(x => x.GlobalRank);

		// Assert
		Assert.Equal(expectedHighestRank, actualHighestRank);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(2)]
	[InlineData(3)]
	public async Task HighestRankAsync_ReturnsCountryRank(int mode)
	{
		// Arrange
		const string country = "US";
		using var context = _fixture.CreateContext();
		var repository = RepositoryInstances.BaseStatsRepository(context);

		// Act
		int expectedHighestRank = await repository.HighestRankAsync(mode, country);
		int actualHighestRank = await context.BaseStats
		                                     .WhereMode(mode)
		                                     .Where(x => x.Player.Country == country)
		                                     .Select(x => x.CountryRank)
		                                     .MaxAsync();

		// Assert
		Assert.Equal(expectedHighestRank, actualHighestRank);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(2)]
	[InlineData(3)]
	public async Task HighestRatingAsync_ReturnsHighestRating(int mode)
	{
		// Arrange
		using var context = _fixture.CreateContext();
		var repository = RepositoryInstances.BaseStatsRepository(context);

		// Act
		double expectedHighestRating = await repository.HighestRatingAsync(mode);
		double actualHighestRating = await context.BaseStats
		                                          .WhereMode(mode)
		                                          .Select(x => x.Rating)
		                                          .MaxAsync();

		// Assert
		Assert.Equal(expectedHighestRating, actualHighestRating);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(2)]
	[InlineData(3)]
	public async Task HighestRatingAsync_ReturnsCountryRating(int mode)
	{
		// Arrange
		const string country = "US";
		using var context = _fixture.CreateContext();
		var repository = RepositoryInstances.BaseStatsRepository(context);

		// Act
		double expectedHighestRating = await repository.HighestRatingAsync(mode, country);
		double actualHighestRating = await context.BaseStats
		                                          .WhereMode(mode)
		                                          .Where(x => x.Player.Country == country)
		                                          .Select(x => x.Rating)
		                                          .MaxAsync();

		// Assert
		Assert.Equal(expectedHighestRating, actualHighestRating);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(2)]
	[InlineData(3)]
	public async Task HighestMatchesAsync_ReturnsHighestMatches(int mode)
	{
		// Arrange
		using var context = _fixture.CreateContext();
		var repository = RepositoryInstances.BaseStatsRepository(context);

		// Act
		int expectedHighestMatches = await repository.HighestMatchesAsync(mode);
		int actualHighestMatches = await context.Players
		                                        .SelectMany(p => p.MatchStats)
		                                        .Where(ms => ms.Match.Tournament.Mode == mode)
		                                        .GroupBy(ms => ms.PlayerId)
		                                        .OrderByDescending(g => g.Count())
		                                        .Select(g => g.Count())
		                                        .FirstOrDefaultAsync();

		// Assert
		Assert.Equal(expectedHighestMatches, actualHighestMatches);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(2)]
	[InlineData(3)]
	public async Task HighestMatchesAsync_ReturnsCountryMatches(int mode)
	{
		// Arrange
		const string country = "US";
		using var context = _fixture.CreateContext();
		var repository = RepositoryInstances.BaseStatsRepository(context);

		// Act
		int expectedHighestMatches = await repository.HighestMatchesAsync(mode, country);
		int actualHighestMatches = await context.Players
		                                        .SelectMany(p => p.MatchStats)
		                                        .Where(ms => ms.Match.Tournament.Mode == mode && ms.Player.Country == country)
		                                        .GroupBy(ms => ms.PlayerId)
		                                        .OrderByDescending(g => g.Count())
		                                        .Select(g => g.Count())
		                                        .FirstOrDefaultAsync();

		// Assert
		Assert.Equal(expectedHighestMatches, actualHighestMatches);
	}
}