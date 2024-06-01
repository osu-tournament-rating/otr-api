using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.Multiplayer;

/// <summary>
/// No description
/// </summary>
/// <remarks>Undocumented</remarks>
/// <copyright>
/// ppy 2024
/// Last accessed May 2024
/// </copyright>
public class MultiplayerMatchInfoJsonModel : JsonModelBase
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("start_time")]
    public DateTimeOffset StartTime { get; set; }

    [JsonProperty("end_time")]
    public DateTimeOffset EndTime { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = null!;
}
