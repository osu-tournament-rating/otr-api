using Common.Enums;
using OsuApiClient.Domain.Osu.Beatmaps;
using OsuApiClient.Domain.Osu.Users;

namespace TestingUtils.SeededData;

/// <summary>
/// Utility for generating OsuApiClient domain objects with seeded data for testing
/// </summary>
public static class SeededOsuApiData
{
    /// <summary>
    /// Creates a <see cref="BeatmapExtended"/> with test data
    /// </summary>
    public static BeatmapExtended CreateBeatmapExtended(
        long beatmapId = 1,
        long beatmapsetId = 1,
        string diffName = "Hard",
        Ruleset ruleset = Ruleset.Osu,
        BeatmapRankedStatus rankedStatus = BeatmapRankedStatus.Ranked,
        double starRating = 5.5)
    {
        return new BeatmapExtended
        {
            Id = beatmapId,
            BeatmapsetId = beatmapsetId,
            DifficultyName = diffName,
            Ruleset = ruleset,
            RankedStatus = rankedStatus,
            StarRating = starRating,
            TotalLength = 180,
            HitLength = 170,
            Bpm = 180,
            CountCircles = 300,
            CountSliders = 150,
            CountSpinners = 10,
            CircleSize = 4,
            HpDrain = 6,
            OverallDifficulty = 8,
            ApproachRate = 9,
            MaxCombo = 800
        };
    }

    /// <summary>
    /// Creates a <see cref="BeatmapsetExtended"/> with test data
    /// </summary>
    public static BeatmapsetExtended CreateBeatmapsetExtended(
        long beatmapsetId = 1,
        string artist = "Test Artist",
        string title = "Test Song",
        long creatorId = 12345,
        string creatorUsername = "TestMapper",
        string creatorCountryCode = "US",
        BeatmapRankedStatus rankedStatus = BeatmapRankedStatus.Ranked,
        params BeatmapExtended[] beatmaps)
    {
        return new BeatmapsetExtended
        {
            Id = beatmapsetId,
            Artist = artist,
            Title = title,
            RankedStatus = rankedStatus,
            RankedDate = DateTime.UtcNow.AddDays(-30),
            SubmittedDate = DateTime.UtcNow.AddDays(-60),
            User = new User
            {
                Id = creatorId,
                Username = creatorUsername,
                CountryCode = creatorCountryCode
            },
            Beatmaps = beatmaps.Length > 0 ? beatmaps : new[]
            {
                CreateBeatmapExtended(1, beatmapsetId, "Easy", Ruleset.Osu, rankedStatus, 2.5),
                CreateBeatmapExtended(2, beatmapsetId, "Normal", Ruleset.Osu, rankedStatus, 3.5),
                CreateBeatmapExtended(3, beatmapsetId, "Hard", Ruleset.Osu, rankedStatus)
            }
        };
    }
}
