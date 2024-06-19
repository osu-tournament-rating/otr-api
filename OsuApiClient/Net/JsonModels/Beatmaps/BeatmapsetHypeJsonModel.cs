using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.Beatmaps;

/// <summary>
/// No description
/// </summary>
/// <remarks>Undocumented</remarks>
/// <copyright>
/// ppy 2024
/// Last accessed June 2024
/// </copyright>
public class BeatmapsetHypeJsonModel
{
    [JsonProperty("current")]
    public int CurrentHype { get; set; }

    [JsonProperty("required")]
    public int RequiredHype { get; set; }
}
