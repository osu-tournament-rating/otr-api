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
        Assert.NotEmpty(tournament.Matches);
    }
}
