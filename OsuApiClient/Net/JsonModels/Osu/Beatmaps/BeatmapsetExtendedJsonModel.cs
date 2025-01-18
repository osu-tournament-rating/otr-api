using Newtonsoft.Json;
using OsuApiClient.Net.JsonModels.Osu.Users;

namespace OsuApiClient.Net.JsonModels.Osu.Beatmaps;

/// <summary>
/// Represents a beatmapset. This extends <see cref="BeatmapsetJsonModel"/> with additional attributes
/// </summary>
/// <copyright>
/// ppy 2025 https://osu.ppy.sh/docs/index.html#beatmapset
/// Last accessed January 2025
/// </copyright>
public class BeatmapsetExtendedJsonModel : BeatmapsetJsonModel
{
    [JsonProperty("beatmaps")]
    public BeatmapExtendedJsonModel[] Beatmaps { get; set; } = [];

    [JsonProperty("related_users")]
    public UserJsonModel[] RelatedUsers { get; set; } = [];

    [JsonProperty("user")]
    public UserJsonModel? User { get; set; }
}
