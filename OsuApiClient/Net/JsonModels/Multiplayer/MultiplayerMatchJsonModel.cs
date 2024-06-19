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
    public MatchInfoJsonModel Match { get; set; } = null!;

    [JsonProperty("events")]
    public MatchEventJsonModel[] Events { get; set; } = [];

    [JsonProperty("users")]
    public MatchUserJsonModel[] Users { get; set; } = [];

    [JsonProperty("first_event_id")]
    public long FirstEventId { get; set; }

    [JsonProperty("latest_event_id")]
    public long LatestEventId { get; set; }

    [JsonProperty("current_game_id")]
    public long? CurrentGameId { get; set; }
}
