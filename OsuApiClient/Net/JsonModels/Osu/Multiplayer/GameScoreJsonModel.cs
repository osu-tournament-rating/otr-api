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
public class GameScoreJsonModel : JsonModelBase
{
    [JsonProperty("accuracy")]
    public double Accuracy { get; set; }

    [JsonProperty("best_id")]
    public long? BestId { get; set; }

    [JsonProperty("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonProperty("id")]
    public long? Id { get; set; }

    [JsonProperty("max_combo")]
    public int MaxCombo { get; set; }

    [JsonProperty("mode")]
    public string Mode { get; set; } = null!;

    [JsonProperty("mode_int")]
    public int ModeInt { get; set; }

    [JsonProperty("mods")]
    public string[] Mods { get; set; } = null!;

    [JsonProperty("passed")]
    public bool Passed { get; set; }

    [JsonProperty("perfect")]
    public int Perfect { get; set; }

    [JsonProperty("pp")]
    public double? Pp { get; set; }

    [JsonProperty("rank")]
    public string Rank { get; set; } = null!;

    [JsonProperty("replay")]
    public bool Replay { get; set; }

    [JsonProperty("score")]
    public long Score { get; set; }

    [JsonProperty("statistics")]
    public GameScoreStatisticsJsonModel Statistics { get; set; } = null!;

    [JsonProperty("type")]
    public string Type { get; set; } = null!;

    [JsonProperty("user_id")]
    public long UserId { get; set; }

    // This field is always {"pin": null}
    // [JsonProperty("current_user_attributes")]
    // public object? CurrentUserAttributes { get; set; }

    [JsonProperty("match")]
    public GameSlotInfoJsonModel Match { get; set; } = null!;
}
