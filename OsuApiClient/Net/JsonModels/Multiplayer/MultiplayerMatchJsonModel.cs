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
public class MultiplayerMatchJsonModel : JsonModelBase
{
    [JsonProperty("match")]
    public MultiplayerMatchInfoJsonModel Match { get; set; } = null!;

    // events

    // users

    [JsonProperty("first_event_id")]
    public long FirstEventId { get; set; }

    [JsonProperty("latest_event_id")]
    public long LatestEventId { get; set; }

    [JsonProperty("current_game_id")]
    public long? CurrentGameId { get; set; }
}
