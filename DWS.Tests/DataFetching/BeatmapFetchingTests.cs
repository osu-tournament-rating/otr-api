using DWS.Tests.DataFetching.TestFixtures;
using Microsoft.EntityFrameworkCore;
using Moq;
using OsuApiClient.Domain.Osu.Beatmaps;
using Beatmap = Database.Entities.Beatmap;
using Beatmapset = Database.Entities.Beatmapset;

namespace DWS.Tests.DataFetching;

public class BeatmapFetchingTests : BeatmapFetchTestBase
{
    [Fact]
    public async Task FetchAndPersistBeatmapAsync_WhenBeatmapExistsInApi_CreatesNewBeatmapWithFullData()
    {
        // Arrange
        const long beatmapId = 1;
        const long beatmapsetId = 1;
        const long creatorId = 1;

        BeatmapExtended apiBeatmap = TestDataBuilders.BeatmapBuilder.CreateBeatmapExtended(
            beatmapId: beatmapId,
            beatmapsetId: beatmapsetId,
            diffName: "Expert",
            starRating: 6.8
        );

        BeatmapsetExtended apiBeatmapset = TestDataBuilders.BeatmapBuilder.CreateBeatmapsetExtended(
            beatmapsetId: beatmapsetId,
            artist: "Test Artist",
            title: "Test Song",
            creatorId: creatorId,
            creatorUsername: "TestMapper",
            beatmaps: new[] { apiBeatmap }
        );

        MockOsuClient.Setup(x => x.GetBeatmapAsync(beatmapId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiBeatmap);

        MockOsuClient.Setup(x => x.GetBeatmapsetAsync(beatmapsetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiBeatmapset);

        // Act
        bool result = await Service.FetchAndPersistBeatmapAsync(beatmapId);

        // Assert
        Assert.True(result);

        // Verify beatmap was created with correct data
        Beatmap? beatmap = await GetEntityAsync<Beatmap>(b => b.OsuId == beatmapId);
        Assert.NotNull(beatmap);
        Assert.True(beatmap.HasData);
        Assert.Equal("Expert", beatmap.DiffName);
        Assert.Equal(6.8, beatmap.Sr);
        Assert.Equal(apiBeatmap.MaxCombo, beatmap.MaxCombo);
        Assert.Equal(apiBeatmap.CountCircles, beatmap.CountCircle);
        Assert.Equal(apiBeatmap.CountSliders, beatmap.CountSlider);
        Assert.Equal(apiBeatmap.CountSpinners, beatmap.CountSpinner);

        // Verify beatmapset was created
        Beatmapset? beatmapset = await Context.Beatmapsets
            .Include(bs => bs.Creator)
            .FirstOrDefaultAsync(bs => bs.OsuId == beatmapsetId);
        Assert.NotNull(beatmapset);
        Assert.Equal("Test Artist", beatmapset.Artist);
        Assert.Equal("Test Song", beatmapset.Title);

        // Verify creator was created
        Assert.NotNull(beatmapset.Creator);
        Assert.Equal(creatorId, beatmapset.Creator.OsuId);
        Assert.Equal("TestMapper", beatmapset.Creator.Username);

        // Verify API calls were made
        MockOsuClient.Verify(x => x.GetBeatmapAsync(beatmapId, It.IsAny<CancellationToken>()), Times.Once);
        MockOsuClient.Verify(x => x.GetBeatmapsetAsync(beatmapsetId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FetchAndPersistBeatmapAsync_WhenBeatmapNotFoundInApi_CreatesPlaceholderWithNoData()
    {
        // Arrange
        const long beatmapId = 1;

        MockOsuClient.Setup(x => x.GetBeatmapAsync(beatmapId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BeatmapExtended?)null);

        // Act
        bool result = await Service.FetchAndPersistBeatmapAsync(beatmapId);

        // Assert
        Assert.False(result);

        // Verify beatmap was created with HasData = false
        Beatmap? beatmap = await GetEntityAsync<Beatmap>(b => b.OsuId == beatmapId);
        Assert.NotNull(beatmap);
        Assert.False(beatmap.HasData);
        Assert.Equal(string.Empty, beatmap.DiffName);
        Assert.Equal(0.0, beatmap.Sr);
        Assert.Null(beatmap.Beatmapset);

        // Verify only beatmap API call was made (not beatmapset)
        MockOsuClient.Verify(x => x.GetBeatmapAsync(beatmapId, It.IsAny<CancellationToken>()), Times.Once);
        MockOsuClient.Verify(x => x.GetBeatmapsetAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task FetchAndPersistBeatmapAsync_WhenBeatmapExistsInDatabaseAndApi_UpdatesExistingData()
    {
        // Arrange
        const long beatmapId = 1;
        const long beatmapsetId = 1;

        // Create existing beatmap in database with old data
        var existingBeatmap = new Beatmap
        {
            OsuId = beatmapId,
            HasData = false,
            DiffName = "Old Name",
            Sr = 3.5
        };

        Context.Beatmaps.Add(existingBeatmap);
        await Context.SaveChangesAsync();

        // Setup API to return updated data
        BeatmapExtended apiBeatmap = TestDataBuilders.BeatmapBuilder.CreateBeatmapExtended(
            beatmapId: beatmapId,
            beatmapsetId: beatmapsetId,
            diffName: "Updated Expert",
            starRating: 7.2
        );

        BeatmapsetExtended apiBeatmapset = TestDataBuilders.BeatmapBuilder.CreateBeatmapsetExtended(
            beatmapsetId: beatmapsetId,
            beatmaps: new[] { apiBeatmap }
        );

        MockOsuClient.Setup(x => x.GetBeatmapAsync(beatmapId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiBeatmap);

        MockOsuClient.Setup(x => x.GetBeatmapsetAsync(beatmapsetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiBeatmapset);

        // Act
        bool result = await Service.FetchAndPersistBeatmapAsync(beatmapId);

        // Assert
        Assert.True(result);

        // Verify beatmap was updated
        Beatmap? updatedBeatmap = await GetEntityAsync<Beatmap>(b => b.OsuId == beatmapId);
        Assert.NotNull(updatedBeatmap);
        Assert.True(updatedBeatmap.HasData);
        Assert.Equal("Updated Expert", updatedBeatmap.DiffName);
        Assert.Equal(7.2, updatedBeatmap.Sr);

        // Verify only one beatmap exists
        int beatmapCount = await CountEntitiesAsync<Beatmap>();
        Assert.Equal(1, beatmapCount);
    }
}
