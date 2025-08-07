using AutoMapper;
using DWS.Configurations;
using DWS.Services;
using DWS.Services.Implementations;
using DWS.Services.Interfaces;
using DWS.Tests.DataFetching.TestFixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OsuApiClient.Domain.Osu.Beatmaps;
using TestingUtils.SeededData;
using Beatmap = Database.Entities.Beatmap;
using Beatmapset = Database.Entities.Beatmapset;
using Player = Database.Entities.Player;

namespace DWS.Tests.DataFetching;

public class BeatmapsetFetchingTests : IntegrationTestBase
{
    private BeatmapsetFetchService BeatmapsetService
    {
        get
        {
            ILogger<BeatmapsetFetchService> logger = CreateLogger<BeatmapsetFetchService>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<DwsMapperProfile>());
            IMapper mapper = config.CreateMapper();
            var mockDataCompletionService = new Mock<ITournamentDataCompletionService>();
            return new BeatmapsetFetchService(logger, Context, BeatmapsRepository, BeatmapsetsRepository, PlayersRepository, MockOsuClient.Object, mockDataCompletionService.Object, mapper);
        }
    }

    [Fact]
    public async Task FetchAndPersistBeatmapsetByBeatmapIdAsync_WhenBeatmapExistsInApi_CreatesEntireBeatmapset()
    {
        // Arrange
        const long beatmapId = 1;
        const long beatmapsetId = 100;
        const long creatorId = 1000;

        // Create multiple beatmaps for the beatmapset
        BeatmapExtended[] beatmaps =
        [
            SeededOsuApiData.CreateBeatmapExtended(
                beatmapId: beatmapId,
                beatmapsetId: beatmapsetId,
                diffName: "Easy",
                starRating: 2.5
            ),
            SeededOsuApiData.CreateBeatmapExtended(
                beatmapId: 2,
                beatmapsetId: beatmapsetId,
                diffName: "Normal",
                starRating: 3.8
            ),
            SeededOsuApiData.CreateBeatmapExtended(
                beatmapId: 3,
                beatmapsetId: beatmapsetId,
                diffName: "Hard",
                starRating: 5.2
            )
        ];

        BeatmapsetExtended apiBeatmapset = SeededOsuApiData.CreateBeatmapsetExtended(
            beatmapsetId: beatmapsetId,
            artist: "Test Artist",
            title: "Test Song",
            creatorId: creatorId,
            creatorUsername: "TestMapper",
            beatmaps: beatmaps
        );

        MockOsuClient.Setup(x => x.GetBeatmapAsync(beatmapId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beatmaps[0]);

        MockOsuClient.Setup(x => x.GetBeatmapsetAsync(beatmapsetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiBeatmapset);

        // Act
        bool result = await BeatmapsetService.FetchAndPersistBeatmapsetAsync(beatmapId);

        // Assert
        Assert.True(result);

        // Verify beatmapset was created
        Beatmapset? beatmapset = await Context.Beatmapsets
            .Include(bs => bs.Creator)
            .Include(bs => bs.Beatmaps)
            .FirstOrDefaultAsync(bs => bs.OsuId == beatmapsetId);

        Assert.NotNull(beatmapset);
        Assert.Equal("Test Artist", beatmapset.Artist);
        Assert.Equal("Test Song", beatmapset.Title);

        // Verify all beatmaps were created
        Assert.Equal(3, beatmapset.Beatmaps.Count);
        Assert.All(beatmapset.Beatmaps, b => Assert.True(b.HasData));

        Beatmap? easyBeatmap = beatmapset.Beatmaps.FirstOrDefault(b => b.DiffName == "Easy");
        Assert.NotNull(easyBeatmap);
        Assert.Equal(2.5, easyBeatmap.Sr);

        // Verify creator was created
        Assert.NotNull(beatmapset.Creator);
        Assert.Equal(creatorId, beatmapset.Creator.OsuId);
        Assert.Equal("TestMapper", beatmapset.Creator.Username);

        // Verify API calls
        MockOsuClient.Verify(x => x.GetBeatmapAsync(beatmapId, It.IsAny<CancellationToken>()), Times.Once);
        MockOsuClient.Verify(x => x.GetBeatmapsetAsync(beatmapsetId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FetchAndPersistBeatmapsetByBeatmapIdAsync_WhenBeatmapAlreadyExistsWithNoData_SkipsApiCalls()
    {
        // Arrange
        const long beatmapId = 1;

        // Create existing beatmap with HasData = false
        var existingBeatmap = new Beatmap
        {
            OsuId = beatmapId,
            HasData = false
        };
        Context.Beatmaps.Add(existingBeatmap);
        await Context.SaveChangesAsync();

        // Act
        bool result = await BeatmapsetService.FetchAndPersistBeatmapsetAsync(beatmapId);

        // Assert
        Assert.False(result);

        // Verify no API calls were made
        MockOsuClient.Verify(x => x.GetBeatmapAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
        MockOsuClient.Verify(x => x.GetBeatmapsetAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task FetchAndPersistBeatmapsetByBeatmapIdAsync_WhenBeatmapNotFoundInApi_CreatesBeatmapWithNoData()
    {
        // Arrange
        const long beatmapId = 1;

        MockOsuClient.Setup(x => x.GetBeatmapAsync(beatmapId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BeatmapExtended?)null);

        // Act
        bool result = await BeatmapsetService.FetchAndPersistBeatmapsetAsync(beatmapId);

        // Assert
        Assert.False(result);

        // Verify beatmap was created with HasData = false
        Beatmap? beatmap = await BeatmapsRepository.GetAsync(beatmapId);
        Assert.NotNull(beatmap);
        Assert.False(beatmap.HasData);

        // Verify only beatmap API call was made
        MockOsuClient.Verify(x => x.GetBeatmapAsync(beatmapId, It.IsAny<CancellationToken>()), Times.Once);
        MockOsuClient.Verify(x => x.GetBeatmapsetAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task FetchAndPersistBeatmapsetByBeatmapIdAsync_WhenBeatmapsetNotFoundInApi_CreatesMinimalBeatmapset()
    {
        // Arrange
        const long beatmapId = 1;
        const long beatmapsetId = 100;

        BeatmapExtended apiBeatmap = SeededOsuApiData.CreateBeatmapExtended(
            beatmapId: beatmapId,
            beatmapsetId: beatmapsetId
        );

        MockOsuClient.Setup(x => x.GetBeatmapAsync(beatmapId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiBeatmap);

        MockOsuClient.Setup(x => x.GetBeatmapsetAsync(beatmapsetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BeatmapsetExtended?)null);

        // Act
        bool result = await BeatmapsetService.FetchAndPersistBeatmapsetAsync(beatmapId);

        // Assert
        Assert.False(result);

        // Verify beatmapset was created
        Beatmapset? beatmapset = await Context.Beatmapsets.FirstOrDefaultAsync(bs => bs.OsuId == beatmapsetId);
        Assert.NotNull(beatmapset);

        // Verify API calls
        MockOsuClient.Verify(x => x.GetBeatmapAsync(beatmapId, It.IsAny<CancellationToken>()), Times.Once);
        MockOsuClient.Verify(x => x.GetBeatmapsetAsync(beatmapsetId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FetchAndPersistBeatmapsetByBeatmapIdAsync_WhenBeatmapsetExistsInDatabase_UpdatesExistingData()
    {
        // Arrange
        const long beatmapId = 1;
        const long beatmapsetId = 100;
        const long creatorId = 1000;

        // Create existing beatmapset with old data
        var existingBeatmapset = new Beatmapset
        {
            OsuId = beatmapsetId,
            Artist = "Old Artist",
            Title = "Old Title"
        };
        Context.Beatmapsets.Add(existingBeatmapset);
        await Context.SaveChangesAsync();

        // Setup API to return updated data
        BeatmapExtended apiBeatmap = SeededOsuApiData.CreateBeatmapExtended(
            beatmapId: beatmapId,
            beatmapsetId: beatmapsetId
        );

        BeatmapsetExtended apiBeatmapset = SeededOsuApiData.CreateBeatmapsetExtended(
            beatmapsetId: beatmapsetId,
            artist: "Updated Artist",
            title: "Updated Title",
            creatorId: creatorId,
            beatmaps: [apiBeatmap]
        );

        MockOsuClient.Setup(x => x.GetBeatmapAsync(beatmapId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiBeatmap);

        MockOsuClient.Setup(x => x.GetBeatmapsetAsync(beatmapsetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiBeatmapset);

        // Act
        bool result = await BeatmapsetService.FetchAndPersistBeatmapsetAsync(beatmapId);

        // Assert
        Assert.True(result);

        // Verify beatmapset was updated
        Beatmapset? updatedBeatmapset = await Context.Beatmapsets.FirstOrDefaultAsync(bs => bs.OsuId == beatmapsetId);
        Assert.NotNull(updatedBeatmapset);
        Assert.Equal("Updated Artist", updatedBeatmapset.Artist);
        Assert.Equal("Updated Title", updatedBeatmapset.Title);

        // Verify only one beatmapset exists
        int beatmapsetCount = await Context.Beatmapsets.CountAsync();
        Assert.Equal(1, beatmapsetCount);
    }

    [Fact]
    public async Task FetchAndPersistBeatmapsetByBeatmapIdAsync_WhenBeatmapExistsWithData_PreservesExistingDataOnApiNull()
    {
        // Arrange
        const long beatmapId = 1;
        const long beatmapsetId = 100;

        // Create existing beatmap with data
        var existingBeatmap = new Beatmap
        {
            OsuId = beatmapId,
            HasData = true,
            DiffName = "Existing Diff",
            Sr = 5.5,
            Beatmapset = new Beatmapset
            {
                OsuId = beatmapsetId,
                Artist = "Existing Artist",
                Title = "Existing Title"
            }
        };
        Context.Beatmaps.Add(existingBeatmap);
        await Context.SaveChangesAsync();

        MockOsuClient.Setup(x => x.GetBeatmapAsync(beatmapId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BeatmapExtended?)null);

        // Act
        bool result = await BeatmapsetService.FetchAndPersistBeatmapsetAsync(beatmapId);

        // Assert
        Assert.False(result);

        // Verify beatmap data was preserved but HasData was set to false
        Beatmap? beatmap = await Context.Beatmaps
            .Include(b => b.Beatmapset)
            .FirstOrDefaultAsync(b => b.OsuId == beatmapId);

        Assert.NotNull(beatmap);
        Assert.False(beatmap.HasData);
        Assert.Equal("Existing Diff", beatmap.DiffName);
        Assert.Equal(5.5, beatmap.Sr);
        Assert.NotNull(beatmap.Beatmapset);
        Assert.Equal("Existing Artist", beatmap.Beatmapset.Artist);
    }

    [Fact]
    public async Task FetchAndPersistBeatmapsetByBeatmapIdAsync_WhenMultipleBeatmapsExist_HandlesTracking()
    {
        // Arrange
        const long requestedBeatmapId = 1;
        const long existingBeatmapId = 2;
        const long beatmapsetId = 100;

        // Create an existing beatmap in the beatmapset
        var existingBeatmap = new Beatmap
        {
            OsuId = existingBeatmapId,
            HasData = true,
            DiffName = "Existing",
            Beatmapset = new Beatmapset
            {
                OsuId = beatmapsetId,
                Artist = "Artist",
                Title = "Title"
            }
        };
        Context.Beatmaps.Add(existingBeatmap);
        await Context.SaveChangesAsync();

        // Setup API to return beatmapset with both beatmaps
        BeatmapExtended[] beatmaps =
        [
            SeededOsuApiData.CreateBeatmapExtended(
                beatmapId: requestedBeatmapId,
                beatmapsetId: beatmapsetId,
                diffName: "New"
            ),
            SeededOsuApiData.CreateBeatmapExtended(
                beatmapId: existingBeatmapId,
                beatmapsetId: beatmapsetId,
                diffName: "Updated Existing"
            )
        ];

        BeatmapsetExtended apiBeatmapset = SeededOsuApiData.CreateBeatmapsetExtended(
            beatmapsetId: beatmapsetId,
            beatmaps: beatmaps
        );

        MockOsuClient.Setup(x => x.GetBeatmapAsync(requestedBeatmapId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beatmaps[0]);

        MockOsuClient.Setup(x => x.GetBeatmapsetAsync(beatmapsetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiBeatmapset);

        // Act
        bool result = await BeatmapsetService.FetchAndPersistBeatmapsetAsync(requestedBeatmapId);

        // Assert
        Assert.True(result);

        // Verify both beatmaps exist and are updated
        Beatmapset? beatmapset = await Context.Beatmapsets
            .Include(bs => bs.Beatmaps)
            .FirstOrDefaultAsync(bs => bs.OsuId == beatmapsetId);

        Assert.NotNull(beatmapset);
        Assert.Equal(2, beatmapset.Beatmaps.Count);

        Beatmap? newBeatmap = beatmapset.Beatmaps.FirstOrDefault(b => b.OsuId == requestedBeatmapId);
        Assert.NotNull(newBeatmap);
        Assert.Equal("New", newBeatmap.DiffName);

        Beatmap? updatedExisting = beatmapset.Beatmaps.FirstOrDefault(b => b.OsuId == existingBeatmapId);
        Assert.NotNull(updatedExisting);
        Assert.Equal("Updated Existing", updatedExisting.DiffName);
    }

    [Fact]
    public async Task FetchAndPersistBeatmapsetByBeatmapIdAsync_WhenExceptionOccurs_ThrowsAndLogsError()
    {
        // Arrange
        const long beatmapId = 1;

        MockOsuClient.Setup(x => x.GetBeatmapAsync(beatmapId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("API error"));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(
            async () => await BeatmapsetService.FetchAndPersistBeatmapsetAsync(beatmapId)
        );

        // Verify no data was persisted
        int beatmapCount = await Context.Beatmaps.CountAsync();
        Assert.Equal(0, beatmapCount);
    }

    [Fact]
    public async Task FetchAndPersistBeatmapsetByBeatmapIdAsync_UpdatesPlayerDataWhenCreatorChanges()
    {
        // Arrange
        const long beatmapId = 1;
        const long beatmapsetId = 100;
        const long creatorId = 1000;

        // Create existing player with old username
        var existingPlayer = new Player
        {
            OsuId = creatorId,
            Username = "OldUsername",
            Country = "US"
        };
        Context.Players.Add(existingPlayer);
        await Context.SaveChangesAsync();

        // Setup API to return updated creator data
        BeatmapExtended apiBeatmap = SeededOsuApiData.CreateBeatmapExtended(
            beatmapId: beatmapId,
            beatmapsetId: beatmapsetId
        );

        BeatmapsetExtended apiBeatmapset = SeededOsuApiData.CreateBeatmapsetExtended(
            beatmapsetId: beatmapsetId,
            creatorId: creatorId,
            creatorUsername: "NewUsername",
            creatorCountryCode: "GB",
            beatmaps: [apiBeatmap]
        );

        MockOsuClient.Setup(x => x.GetBeatmapAsync(beatmapId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiBeatmap);

        MockOsuClient.Setup(x => x.GetBeatmapsetAsync(beatmapsetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiBeatmapset);

        // Act
        bool result = await BeatmapsetService.FetchAndPersistBeatmapsetAsync(beatmapId);

        // Assert
        Assert.True(result);

        // Verify player was updated
        Player? updatedPlayer = await PlayersRepository.GetAsync([creatorId]).ContinueWith(t => t.Result.FirstOrDefault());
        Assert.NotNull(updatedPlayer);
        Assert.Equal("NewUsername", updatedPlayer.Username);
        Assert.Equal("GB", updatedPlayer.Country);

        // Verify only one player exists
        int playerCount = await Context.Players.CountAsync();
        Assert.Equal(1, playerCount);
    }
}
