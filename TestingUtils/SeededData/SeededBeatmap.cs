using Common.Enums;
using Database.Entities;

namespace TestingUtils.SeededData;

/// <summary>
/// Utility for generating <see cref="Beatmap"/>s with seeded data
/// </summary>
public static class SeededBeatmap
{
    private const int MaxComboMin = 150;
    public const int MaxComboMax = 3000;

    private const double SrMin = 2;
    private const double SrMax = 10;

    private const double BpmMin = 10;
    private const double BpmMax = 400;

    private const int LengthMin = 30;
    private const int LengthMax = 600;

    private static readonly Random _sRand = new();

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
        Beatmapset? beatmapset = null
    )
    {
        var seededBeatmap = new Beatmap
        {
            Id = id ?? _sRand.Next(),
            OsuId = osuId ?? _sRand.NextInt64(),
            HasData = hasData ?? _sRand.NextBool(),
            DiffName = diffName ?? string.Empty,
            RankedStatus = rankedStatus ?? _sRand.NextEnum<BeatmapRankedStatus>(),
            TotalLength = totalLength ?? _sRand.NextInt64(LengthMin, LengthMax),
            Sr = sr ?? _sRand.NextDouble(SrMin, SrMax),
            Bpm = bpm ?? _sRand.NextDouble(BpmMin, BpmMax),
            Cs = cs ?? _sRand.NextDouble(10),
            Ar = ar ?? _sRand.NextDouble(11),
            Hp = hp ?? _sRand.NextDouble(10),
            Od = od ?? _sRand.NextDouble(11),
            Ruleset = ruleset ?? _sRand.NextEnum<Ruleset>(),
            MaxCombo = maxCombo ?? _sRand.NextInclusive(MaxComboMin, MaxComboMax),
        };

        beatmapset ??= new Beatmapset
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

        seededBeatmap.Beatmapset = beatmapset;
        seededBeatmap.CountCircle = circleCount ?? _sRand.NextInclusive(seededBeatmap.MaxCombo.Value);
        seededBeatmap.CountSlider = sliderCount ?? _sRand.NextInclusive(seededBeatmap.MaxCombo.Value - seededBeatmap.CountCircle);
        seededBeatmap.CountSpinner = spinnerCount ?? seededBeatmap.MaxCombo.Value - seededBeatmap.CountCircle - seededBeatmap.CountSlider;

        return seededBeatmap;
    }
}
