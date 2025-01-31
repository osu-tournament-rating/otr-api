using Database.Entities;
using Database.Enums;

namespace TestingUtils.SeededData;

/// <summary>
/// Utility for generating <see cref="Beatmap"/>s with seeded data
/// </summary>
public static class SeededBeatmap
{
    public const int MaxComboMin = 150;
    public const int MaxComboMax = 3000;

    public const double SrMin = 2;
    public const double SrMax = 10;

    public const double BpmMin = 10;
    public const double BpmMax = 400;

    public const int LengthMin = 30;
    public const int LengthMax = 600;

    private static readonly Random s_rand = new();

    /// <summary>
    /// Generates a <see cref="Beatmap"/> with seeded data
    /// </summary>
    /// <remarks>Any properties not given will be randomized</remarks>
    public static Beatmap Generate(
        int? id = null,
        long? osuId = null,
        bool? hasData = null,
        string? artist = null,
        string? title = null,
        string? diffName = null,
        BeatmapRankedStatus? rankedStatus = null,
        double? sr = null,
        double? bpm = null,
        double? cs = null,
        double? ar = null,
        double? hp = null,
        double? od = null,
        int? totalLength = null,
        Ruleset? ruleset = null,
        int? circleCount = null,
        int? sliderCount = null,
        int? spinnerCount = null,
        int? maxCombo = null,
        BeatmapSet? beatmapSet = null
    )
    {
        var seededBeatmap = new Beatmap
        {
            Id = id ?? s_rand.Next(),
            OsuId = osuId ?? s_rand.NextInt64(),
            HasData = hasData ?? s_rand.NextBool(),
            DiffName = diffName ?? string.Empty,
            RankedStatus = rankedStatus ?? s_rand.NextEnum<BeatmapRankedStatus>(),
            TotalLength = totalLength ?? s_rand.NextInt64(LengthMin, LengthMax),
            Sr = sr ?? s_rand.NextDouble(SrMin, SrMax),
            Bpm = bpm ?? s_rand.NextDouble(BpmMin, BpmMax),
            Cs = cs ?? s_rand.NextDouble(10),
            Ar = ar ?? s_rand.NextDouble(11),
            Hp = hp ?? s_rand.NextDouble(10),
            Od = od ?? s_rand.NextDouble(11),
            Ruleset = ruleset ?? s_rand.NextEnum<Ruleset>(),
            MaxCombo = maxCombo ?? s_rand.NextInclusive(MaxComboMin, MaxComboMax),
        };

        beatmapSet ??= new BeatmapSet
        {
            Id = 0,
            Created = default,
            Updated = null,
            OsuId = 0,
            CreatorId = 0,
            Artist = artist ?? "Example Artist",
            Title = title ?? "Example Title",
            RankedStatus = BeatmapRankedStatus.Pending,
            RankedDate = null,
            SubmittedDate = null,
            Creator = null,
            Beatmaps = [seededBeatmap]
        };

        seededBeatmap.BeatmapSet = beatmapSet;
        seededBeatmap.CountCircle = circleCount ?? s_rand.NextInclusive(seededBeatmap.MaxCombo.Value);
        seededBeatmap.CountSlider = sliderCount ?? s_rand.NextInclusive(seededBeatmap.MaxCombo.Value - seededBeatmap.CountCircle);
        seededBeatmap.CountSpinner = spinnerCount ?? seededBeatmap.MaxCombo.Value - seededBeatmap.CountCircle - seededBeatmap.CountSlider;

        return seededBeatmap;
    }
}
