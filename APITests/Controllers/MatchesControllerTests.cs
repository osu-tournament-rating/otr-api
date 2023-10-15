using API;
using API.Controllers;
using API.Enums;
using API.Services.Implementations;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace APITests.Controllers;

[Collection("DatabaseCollection")]
public class MatchesControllerTests
{
	private readonly TestDatabaseFixture _fixture;
	private readonly Mock<ILogger<OsuMatchesController>> _loggerMock;
	private readonly Mock<ILogger<MatchesService>> _matchesLoggerMock;
	private readonly Mock<ILogger<TournamentsService>> _tournamentsLoggerMock;
	private readonly Mock<IMapper> _mapperMock;

	public MatchesControllerTests(TestDatabaseFixture testDatabaseFixture)
	{
		_loggerMock = new Mock<ILogger<OsuMatchesController>>();
		_matchesLoggerMock = new Mock<ILogger<MatchesService>>();
		_tournamentsLoggerMock = new Mock<ILogger<TournamentsService>>();
		_mapperMock = new Mock<IMapper>();
		
		_fixture = testDatabaseFixture;
	}
	
	[Fact]
	public async Task WebSubmissionFlow_Valid()
	{
		// arrange
		using var context = _fixture.CreateContext();
		var controller = OsuMatchesController(context);

		/**
		 * Flow:
		 *
		 * 1. User submits a batch of links to the front-end, along with other tournament information
		 * 2. The front-end sends a POST request to the API with the batch of links
		 * 3. The API validates the links and returns a 200 OK response if everything is good
		 */

		var dummyUserId = (await context.Users.FirstAsync()).Id;
		var batch = new BatchWrapper
		{
			TournamentName = "My Special Tournament",
			Abbreviation = "MST",
			ForumPost = "123.ez.com",
			RankRangeLowerBound = 500,
			Mode = 0,
			TeamSize = 4,
			Ids = new List<long>
			{
				13242343242,
				242342343242,
				4564645646234
			},
			SubmitterId = dummyUserId
		};

		// act
		await context.Database.BeginTransactionAsync();
		var result = await controller.PostAsync(batch); // NOT sending as verified

		// Ensures results are not committed to the database
		context.ChangeTracker.Clear();
		
		// At this point, tournament data should be stored in the database and all matches should be marked as pending.
		var tournament = await context.Tournaments.FirstAsync(x => x.Name == "My Special Tournament");
		var matches = await context.Matches
		                            .Include(x => x.Games)
		                            .ThenInclude(x => x.MatchScores)
		                            .Where(x => x.TournamentId == tournament.Id).ToListAsync();


		// assert
		Assert.Equal(typeof(OkResult), result.GetType());
		
		Assert.NotNull(tournament);
		Assert.Equal("My Special Tournament", tournament.Name);
		Assert.Equal("MST", tournament.Abbreviation);
		Assert.Equal("123.ez.com", tournament.ForumUrl);
		Assert.Equal(500, tournament.RankRangeLowerBound);
		Assert.Equal(0, tournament.Mode);
		Assert.Equal(4, tournament.TeamSize);
		
		Assert.NotNull(matches);
		Assert.Equal(3, matches.Count);
		Assert.Equal((int)MatchVerificationStatus.PendingVerification, matches[0].VerificationStatus);
		Assert.Equal(13242343242, matches[0].MatchId);
		Assert.Equal(tournament.Id, matches[0].TournamentId);
		Assert.Equal(dummyUserId, matches[0].SubmitterUserId);
	}

	private OsuMatchesController OsuMatchesController(OtrContext context)
	{
		var matchesService = new MatchesService(_matchesLoggerMock.Object, _mapperMock.Object, context);
		var tournamentsService = new TournamentsService(_tournamentsLoggerMock.Object, context, matchesService);

		var controller = new OsuMatchesController(_loggerMock.Object, matchesService, tournamentsService);
		return controller;
	}
}