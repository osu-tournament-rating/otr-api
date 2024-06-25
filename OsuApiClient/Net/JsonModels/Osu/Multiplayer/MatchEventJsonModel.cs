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
public class MatchEventJsonModel : JsonModelBase
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("detail")]
    public MatchEventDetailJsonModel Detail { get; set; } = null!;

    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonProperty("user_id")]
    public long? UserId { get; set; }

    [JsonProperty("game")]
    public MultiplayerGameJsonModel? Game { get; set; }
}
