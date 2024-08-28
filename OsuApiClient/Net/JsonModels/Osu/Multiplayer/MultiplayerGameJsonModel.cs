using Newtonsoft.Json;
using OsuApiClient.Net.JsonModels.Osu.Beatmaps;

namespace OsuApiClient.Net.JsonModels.Osu.Multiplayer;

/// <summary>
/// No description
/// </summary>
/// <remarks>Undocumented</remarks>
/// <copyright>
/// ppy 2024
/// Last accessed June 2024
/// </copyright>
public class MultiplayerGameJsonModel : JsonModelBase
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("beatmap_id")]
    public long BeatmapId { get; set; }

    [JsonProperty("start_time")]
    public DateTime StartTime { get; set; }

    [JsonProperty("end_time")]
    public DateTime? EndTime { get; set; }

    [JsonProperty("ruleset")]
    public string Mode { get; set; } = null!;

    [JsonProperty("mode_int")]
    public int ModeInt { get; set; }

    [JsonProperty("scoring_type")]
    public string ScoringType { get; set; } = null!;

    [JsonProperty("team_type")]
    public string TeamType { get; set; } = null!;

    [JsonProperty("mods")]
    public string[] Mods { get; set; } = null!;

    [JsonProperty("beatmap")]
    public BeatmapJsonModel? Beatmap { get; set; }

    [JsonProperty("scores")]
    public GameScoreJsonModel[] Scores { get; set; } = [];
}
