using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.Osu.Multiplayer;

/// <summary>
/// No description
/// </summary>
/// <remarks>Undocumented</remarks>
/// <copyright>
/// ppy 2024
/// Last accessed May 2024
/// </copyright>
public class MatchInfoJsonModel
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("start_time")]
    public DateTime StartTime { get; set; }

    [JsonProperty("end_time")]
    public DateTime? EndTime { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = null!;
}
