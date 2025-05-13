using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;
using Database.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using TestingUtils.SeededData;

namespace Database.Tests.Repositories;

public class TournamentsRepositoryTests
{
    [Fact]
    public async Task CreateAsync_CreatesUserSuccessfully()
    {
        // Arrange
        await using OtrContext context = DatabaseFixture.CreateContext();

        var repo = new TournamentsRepository(context, null!);
        Tournament tournament = SeededTournament.Generate();

        // Act
        await repo.CreateAsync(tournament);

        context.ChangeTracker.Clear();
        Tournament? dbEntry = await context.Tournaments.FindAsync(tournament.Id);

        // Assert
        Assert.NotNull(dbEntry);
    }

    [Fact]
    public async Task CreateAsync_Fails_WhenSameNameAndRulesetExists()
    {
        // Arrange
        await using OtrContext context = DatabaseFixture.CreateContext();
        var repo = new TournamentsRepository(context, null!);

        // Act
        Tournament? existingTournament = await context.Tournaments.FirstOrDefaultAsync();

        // Assert
        Assert.NotNull(existingTournament);

        await Assert.ThrowsAsync<DbUpdateException>(() => repo.CreateAsync(new Tournament
        {
            Name = existingTournament.Name,
            Ruleset = existingTournament.Ruleset
        }));
    }

    [Fact]
    public async Task CreateAsync_CreatesChildren_WhenProvided()
    {
        // Arrange
        await using OtrContext context = DatabaseFixture.CreateContext();
        var repo = new TournamentsRepository(context, new BeatmapsRepository(context));

        var beatmapCreatorId = (await context.Players.FirstAsync()).Id;
        Beatmap beatmap = SeededBeatmap.Generate(creatorPlayerId: beatmapCreatorId);
        Match match = SeededMatch.Generate();

        // Act
        Tournament tournament = SeededTournament.Generate();
        tournament.PooledBeatmaps = [beatmap];
        tournament.Matches = [match];

        await repo.CreateAsync(tournament);
        context.ChangeTracker.Clear();

        Tournament? dbEntry = await context.Tournaments
            .Include(t => t.Matches)
            .Include(t => t.PooledBeatmaps)
            .FirstOrDefaultAsync(t => t.Id == tournament.Id);

        // Assert
        Assert.NotNull(dbEntry);
        Assert.Single(dbEntry.Matches);
        Assert.Single(dbEntry.PooledBeatmaps);
    }

    [Fact]
    public async Task GetAsync_DoesNotReturnNavigationData_ByDefault()
    {
        // Arrange
        await using OtrContext context = DatabaseFixture.CreateContext();
        var repo = new TournamentsRepository(context, new BeatmapsRepository(context));

        // Act
        var id = (await context.Tournaments.FirstAsync()).Id;
        Tournament? baseTournament = await repo.GetAsync(id);

        // Assert
        Assert.NotNull(baseTournament);
        Assert.Null(baseTournament.SubmittedByUser);
        Assert.Null(baseTournament.VerifiedByUser);
        Assert.Empty(baseTournament.Matches);
        Assert.Empty(baseTournament.PooledBeatmaps);
    }

    [Fact]
    public async Task GetAsync_ReturnsNavigationData_WhenEagerLoaded()
    {
        // Arrange
        await using OtrContext context = DatabaseFixture.CreateContext();
        var repo = new TournamentsRepository(context, new BeatmapsRepository(context));

        // Act
        var id = (await context.Tournaments
            .Where(t => t.VerificationStatus == VerificationStatus.Verified && t.PooledBeatmaps.Count > 0)
            .FirstAsync()).Id;

        Tournament? tournament = await repo.GetAsync(id, eagerLoad: true);

        // Assert
        Assert.NotNull(tournament);
        Assert.NotNull(tournament.SubmittedByUser);
        Assert.NotNull(tournament.SubmittedByUser.Player);
        Assert.NotNull(tournament.VerifiedByUser);
        Assert.NotNull(tournament.VerifiedByUser.Player);
        Assert.NotEmpty(tournament.Matches);
        Assert.NotEmpty(tournament.PooledBeatmaps);

        // Check that matches have games loaded
        Match match = tournament.Matches.First();
        Assert.NotEmpty(match.Games);

        // Check that games have scores and beatmaps loaded
        Game game = match.Games.First();
        Assert.NotEmpty(game.Scores);
        Assert.NotNull(game.Beatmap);

        // Check that beatmap has beatmapset and creator loaded
        Assert.NotNull(game.Beatmap!.Beatmapset);
        Assert.NotNull(game.Beatmap!.Beatmapset!.Creator);

        // Check that beatmap has creators loaded
        Assert.NotNull(game.Beatmap!.Creators);
    }

    [Fact]
    public async Task GetVerifiedAsync_ReturnsOnlyVerifiedEntities_WithNavigations()
    {
        // Arrange
        await using OtrContext context = DatabaseFixture.CreateContext();
        var repo = new TournamentsRepository(context, new BeatmapsRepository(context));

        // Act
        var id = (await context.Tournaments
            .Where(t => t.VerificationStatus == VerificationStatus.Verified)
            .FirstAsync()).Id;

        Tournament? tournament = await repo.GetVerifiedAsync(id);

        // Assert
        Assert.NotNull(tournament);
        Assert.Equal(VerificationStatus.Verified, tournament.VerificationStatus);

        // Check navigation properties are loaded
        Assert.NotNull(tournament.SubmittedByUser);
        Assert.NotNull(tournament.SubmittedByUser!.Player);
        Assert.NotNull(tournament.VerifiedByUser);
        Assert.NotNull(tournament.VerifiedByUser!.Player);

        // Check that only verified matches with done processing are included
        Assert.All(tournament.Matches, match =>
        {
            Assert.Equal(VerificationStatus.Verified, match.VerificationStatus);
            Assert.Equal(MatchProcessingStatus.Done, match.ProcessingStatus);

            // Check that only verified games with done processing are included
            Assert.All(match.Games, game =>
            {
                Assert.Equal(VerificationStatus.Verified, game.VerificationStatus);
                Assert.Equal(GameProcessingStatus.Done, game.ProcessingStatus);
                Assert.NotNull(game.Beatmap);

                // Check that only verified scores with done processing are included
                Assert.All(game.Scores, score =>
                {
                    Assert.Equal(VerificationStatus.Verified, score.VerificationStatus);
                    Assert.Equal(ScoreProcessingStatus.Done, score.ProcessingStatus);
                    Assert.NotNull(score.Player);
                });
            });
        });
    }

    [Fact]
    public async Task GetNeedingProcessingAsync_ReturnsCorrectTournamentsWithNavigations()
    {
        // Arrange
        await using OtrContext context = DatabaseFixture.CreateContext();
        var repo = new TournamentsRepository(context, new BeatmapsRepository(context));

        // Set up some tournaments with different processing statuses
        List<Tournament> tournaments = await context.Tournaments.Take(3).ToListAsync();
        tournaments[0].ProcessingStatus = TournamentProcessingStatus.NeedsApproval;
        tournaments[0].LastProcessingDate = DateTime.MinValue.AddDays(3).ToUniversalTime();

        tournaments[1].ProcessingStatus = TournamentProcessingStatus.NeedsAutomationChecks;
        tournaments[1].LastProcessingDate = DateTime.MinValue.AddDays(2).ToUniversalTime();

        tournaments[2].ProcessingStatus = TournamentProcessingStatus.Done;
        tournaments[2].LastProcessingDate = DateTime.MinValue.AddDays(1).ToUniversalTime();

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        // Act
        const int limit = 3;
        var result = (await repo.GetNeedingProcessingAsync(limit)).ToList();

        // Assert
        Assert.NotEmpty(result);
        Assert.True(result.Count == limit);

        // Verify only tournaments needing processing are returned
        Assert.All(result, t =>
        {
            Assert.NotEqual(TournamentProcessingStatus.Done, t.ProcessingStatus);
            Assert.NotEqual(TournamentProcessingStatus.NeedsApproval, t.ProcessingStatus);
        });

        // Verify ordering by LastProcessingDate
        for (var i = 0; i < result.Count - 1; i++)
        {
            Assert.True(result[i].LastProcessingDate <= result[i + 1].LastProcessingDate);
        }

        // Check that navigation properties are loaded
        foreach (Tournament tournament in result)
        {
            // Check pooled beatmaps
            Assert.NotNull(tournament.PooledBeatmaps);

            // Check player tournament stats
            Assert.NotNull(tournament.PlayerTournamentStats);

            // Check matches and their navigations
            Assert.NotNull(tournament.Matches);
            foreach (Match match in tournament.Matches)
            {
                // Check match rosters
                Assert.NotNull(match.Rosters);

                // Check player match stats and their players
                Assert.NotNull(match.PlayerMatchStats);
                foreach (PlayerMatchStats stat in match.PlayerMatchStats)
                {
                    Assert.NotNull(stat.Player);
                }

                // Check player rating adjustments
                Assert.NotNull(match.PlayerRatingAdjustments);

                // Check games and their navigations
                Assert.NotNull(match.Games);
                foreach (Game game in match.Games)
                {
                    // Check game beatmap
                    Assert.NotNull(game.Beatmap);

                    // Check game scores and their players
                    Assert.NotNull(game.Scores);
                    foreach (GameScore score in game.Scores)
                    {
                        Assert.NotNull(score.Player);
                    }

                    // Check game rosters
                    Assert.NotNull(game.Rosters);
                }
            }
        }
    }

}
