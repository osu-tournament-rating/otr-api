using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.Osu.Beatmaps;

/// <summary>
/// Represent a beatmap. This extends <see cref="BeatmapJsonModel"/> with additional attributes
/// </summary>
/// <copyright>
/// ppy 2024 https://osu.ppy.sh/docs/index.html#beatmapextended
/// Last accessed June 2024
/// </copyright>
[SuppressMessage("ReSharper", "CommentTypo")]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class BeatmapExtendedJsonModel : BeatmapJsonModel
{
    [JsonProperty("accuracy")]
    public double Accuracy { get; set; }

    [JsonProperty("ar")]
    public double ApproachRate { get; set; }

    [JsonProperty("bpm")]
    public double Bpm { get; set; }

    [JsonProperty("convert")]
    public bool Convert { get; set; }

    [JsonProperty("count_circles")]
    public int CountCircles { get; set; }

    [JsonProperty("count_sliders")]
    public int CountSliders { get; set; }

    [JsonProperty("count_spinners")]
    public int CountSpinners { get; set; }

    [JsonProperty("cs")]
    public double CircleSize { get; set; }

    [JsonProperty("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [JsonProperty("drain")]
    public double Drain { get; set; }

    [JsonProperty("hit_length")]
    public int HitLength { get; set; }

    [JsonProperty("is_scoreable")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public bool IsScoreable { get; set; }

    [JsonProperty("last_updated")]
    public DateTime LastUpdated { get; set; }

    [JsonProperty("max_combo")]
    public int MaxCombo { get; set; }

    [JsonProperty("passcount")]
    public long PassCount { get; set; }

    [JsonProperty("playcount")]
    public long PlayCount { get; set; }

    [JsonProperty("ranked")]
    public int RankedStatus { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; } = null!;

    [JsonProperty("checksum")]
    public string? Checksum { get; set; }

    // Not really necessary to include
    // [JsonProperty("failtimes")]
    // public BeatmapFailTimes? { get; set; }
}
