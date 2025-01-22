using Database.Entities;
using Database.Enums;

namespace APITests.SeedData;

public static class SeededBeatmapSet
{
    public static BeatmapSet Get() =>
        new()
        {
            OsuId = 123456,
            CreatorId = 1,
            Artist = "Example Artist",
            Title = "Example Title",
            RankedStatus = BeatmapRankedStatus.Ranked,
            RankedDate = new DateTime(2023, 09, 03),
            SubmittedDate = new DateTime(2023, 09, 01),
            Creator = SeededPlayer.Get(),
            Beatmaps = [SeededBeatmap.Get()]
        };
}
