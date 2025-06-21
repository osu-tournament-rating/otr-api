using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.OsuTrack;

/// <summary>
/// No description
/// </summary>
/// <remarks>
/// See <a href="https://github.com/Ameobea/osutrack-api?tab=readme-ov-file#returns-1">Returns</a>
/// </remarks>
public class UserStatUpdateJsonModel
{
    [JsonProperty("count300")]
    public int Count300 { get; set; }

    [JsonProperty("count100")]
    public int Count100 { get; set; }

    [JsonProperty("count50")]
    public int Count50 { get; set; }

    [JsonProperty("playcount")]
    public int PlayCount { get; set; }

    [JsonProperty("ranked_score")]
    public string RankedScore { get; set; } = null!;

    [JsonProperty("total_score")]
    public string TotalScore { get; set; } = null!;

    [JsonProperty("pp_rank")]
    public int Rank { get; set; }

    [JsonProperty("level")]
    public double Level { get; set; }

    [JsonProperty("pp_raw")]
    public double Pp { get; set; }

    [JsonProperty("accuracy")]
    public double Accuracy { get; set; }

    [JsonProperty("count_rank_ss")]
    public int CountSs { get; set; }

    [JsonProperty("count_rank_s")]
    public int CountS { get; set; }

    [JsonProperty("count_rank_a")]
    public int CountA { get; set; }

    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; }
}

