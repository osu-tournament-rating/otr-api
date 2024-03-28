using API.Entities;
using API.Repositories.Interfaces;
using APITests.SeedData;
using Moq;

namespace APITests.Framework.MockRepositories;

public class MockBeatmapRepository : Mock<IBeatmapRepository>
{
    public MockBeatmapRepository()
    {
        SetupAll();
    }

    public MockBeatmapRepository SetupAll() =>
        SetupGet()
        .SetupGetId();

    public MockBeatmapRepository SetupGet()
    {
        Setup(x => x.GetAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) =>
            {
                Beatmap beatmap = SeededBeatmap.Get();
                beatmap.Id = id;

                return beatmap;
            });

        return this;
    }

    public MockBeatmapRepository SetupGetId()
    {
        Setup(x => x.GetIdAsync(It.IsAny<long>()))
            .ReturnsAsync(() =>
            {
                Beatmap beatmap = SeededBeatmap.Get();
                return beatmap.Id;
            });

        return this;
    }
}
