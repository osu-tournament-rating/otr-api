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
        long? mapperId = null,
        string? mapperName = null,
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
        double? length = null,
        Ruleset? ruleset = null,
        int? circleCount = null,
        int? sliderCount = null,
        int? spinnerCount = null,
        int? maxCombo = null
    )
    {
        var seededBeatmap = new Beatmap
        {
            Id = id ?? s_rand.Next(),
            OsuId = osuId ?? s_rand.NextInt64(),
            HasData = hasData ?? s_rand.NextBool(),
            MapperId = mapperId ?? s_rand.NextInt64(),
            MapperName = mapperName ?? string.Empty,
            Artist = artist ?? string.Empty,
            Title = title ?? string.Empty,
            DiffName = diffName ?? string.Empty,
            RankedStatus = rankedStatus ?? s_rand.NextEnum<BeatmapRankedStatus>(),
            Sr = sr ?? s_rand.NextDouble(SrMin, SrMax),
            Bpm = bpm ?? s_rand.NextDouble(BpmMin, BpmMax),
            Cs = cs ?? s_rand.NextDouble(10),
            Ar = ar ?? s_rand.NextDouble(11),
            Hp = hp ?? s_rand.NextDouble(10),
            Od = od ?? s_rand.NextDouble(11),
            Length = length ?? s_rand.NextInclusive(LengthMin, LengthMax),
            Ruleset = ruleset ?? s_rand.NextEnum<Ruleset>(),
            MaxCombo = maxCombo ?? s_rand.NextInclusive(MaxComboMin, MaxComboMax)
        };

        seededBeatmap.CircleCount = circleCount ?? s_rand.NextInclusive(seededBeatmap.MaxCombo);
        seededBeatmap.SliderCount = sliderCount ?? s_rand.NextInclusive(seededBeatmap.MaxCombo - seededBeatmap.CircleCount);
        seededBeatmap.SpinnerCount = spinnerCount ?? seededBeatmap.MaxCombo - seededBeatmap.CircleCount - seededBeatmap.SliderCount;

        return seededBeatmap;
    }
}
