using System.Diagnostics.CodeAnalysis;
using Database.Enums;

namespace OsuApiClient.Domain.Osu.Beatmaps;

/// <summary>
/// Represents a <see cref="Beatmap"/> with additional attributes
/// </summary>
public class BeatmapExtended : Beatmap
{
    /// <summary>
    /// Beats per minute
    /// </summary>
    public double Bpm { get; init; }

    /// <summary>
    /// Circle count
    /// </summary>
    public int CountCircles { get; init; }

    /// <summary>
    /// Slider count
    /// </summary>
    public int CountSliders { get; init; }

    /// <summary>
    /// Spinner count
    /// </summary>
    public int CountSpinners { get; init; }

    /// <summary>
    /// Circle size
    /// </summary>
    public double CircleSize { get; init; }

    /// <summary>
    /// Hp Drain
    /// </summary>
    public double HpDrain { get; init; }

    /// <summary>
    /// Overall difficulty
    /// </summary>
    public double OverallDifficulty { get; init; }

    /// <summary>
    /// Approach rate
    /// </summary>
    public double ApproachRate { get; init; }

    /// <summary>
    /// Max achievable combo
    /// </summary>
    public int? MaxCombo { get; init; }

    /// <summary>
    /// Denotes if the beatmap was converted from another <see cref="Database.Enums.Ruleset"/>
    /// </summary>
    public bool Convert { get; init; }

    /// <summary>
    /// Timestamp of deletion
    /// </summary>
    public DateTime? DeletedAt { get; init; }

    /// <summary>
    /// No description
    /// </summary>
    public int HitLength { get; init; }

    /// <summary>
    /// Denotes if the beatmap is able to grant score
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public bool IsScoreable { get; init; }

    /// <summary>
    /// Timestamp for the last update to the beatmap
    /// </summary>
    public DateTime LastUpdated { get; init; }

    /// <summary>
    /// Pass count
    /// </summary>
    public long PassCount { get; init; }

    /// <summary>
    /// Play count
    /// </summary>
    public long PlayCount { get; init; }

    /// <summary>
    /// Ranked status
    /// </summary>
    public BeatmapRankedStatus RankedStatus { get; init; }

    /// <summary>
    /// osu! website url for the beatmap
    /// </summary>
    public string Url { get; init; } = null!;

    /// <summary>
    /// Checksum
    /// </summary>
    public string? Checksum { get; init; }
}
