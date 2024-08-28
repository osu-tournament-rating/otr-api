using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.Osu.Beatmaps;

/// <summary>
/// Represent a beatmap
/// </summary>
/// <copyright>
/// ppy 2024 https://osu.ppy.sh/docs/index.html#beatmap
/// Last accessed June 2024
/// </copyright>
public class BeatmapJsonModel : JsonModelBase
{
    [JsonProperty("beatmapset_id")]
    public long BeatmapsetId { get; set; }

    [JsonProperty("difficulty_rating")]
    public double DifficultyRating { get; set; }

    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("ruleset")]
    public string Mode { get; set; } = null!;

    [JsonProperty("status")]
    public string Status { get; set; } = null!;

    [JsonProperty("total_length")]
    public long TotalLength { get; set; }

    [JsonProperty("user_id")]
    public long UserId { get; set; }

    [JsonProperty("version")]
    public string Version { get; set; } = null!;

    [JsonProperty("beatmapset")]
    public BeatmapsetJsonModel? Beatmapset { get; set; }
}
