using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.Osu.Multiplayer;

/// <summary>
/// No description
/// </summary>
/// <remarks>Undocumented</remarks>
/// <copyright>
/// ppy 2024
/// Last accessed June 2024
/// </copyright>
public class MatchEventDetailJsonModel
{
    [JsonProperty("type")]
    public string Type { get; set; } = null!;

    [JsonProperty("text")]
    public string? Text { get; set; }
}
