using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.Osu.Beatmaps;

/// <summary>
/// No description
/// </summary>
/// <remarks>Undocumented</remarks>
/// <copyright>
/// ppy 2025
/// Last accessed January 2025
/// </copyright>
public class BeatmapOwnerJsonModel : JsonModelBase
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; } = string.Empty;
}
