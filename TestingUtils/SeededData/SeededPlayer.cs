using Common.Enums.Enums;
using Database.Entities;

namespace TestingUtils.SeededData;

/// <summary>
/// Utility for generating <see cref="Player"/>s with seeded data
/// </summary>
public static class SeededPlayer
{
    private static readonly Random s_rand = new();

    /// <summary>
    /// Generates a <see cref="Player"/> with seeded data
    /// </summary>
    /// <remarks>Any properties not given will be randomized</remarks>
    public static Player Generate(
        int? id = null,
        long? osuId = null,
        string? username = null,
        string? country = null,
        Ruleset? ruleset = null,
        DateTime? osuLastFetch = null,
        DateTime? osuTrackLastFetch = null
    ) =>
        new()
        {
            Id = id ?? s_rand.Next(),
            OsuId = osuId ?? s_rand.NextInt64(),
            Username = username ?? string.Empty,
            Country = country ?? string.Empty,
            DefaultRuleset = ruleset ?? Ruleset.Osu,
            OsuLastFetch = osuLastFetch ?? SeededDate.Placeholder,
            OsuTrackLastFetch = osuTrackLastFetch ?? SeededDate.Placeholder
        };
}
