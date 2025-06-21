using System.Diagnostics.CodeAnalysis;
using Common.Enums;
using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.Osu.Users.Attributes;

/// <summary>
/// A summary of various gameplay statistics for a User. Specific to a Ruleset
/// </summary>
/// <copyright>
/// ppy 2024 https://osu.ppy.sh/docs/index.html#userstatistics
/// Last accessed June 2024
/// </copyright>
[SuppressMessage("ReSharper", "CommentTypo")]
public class UserStatisticsJsonModel
{
    [JsonProperty("count_300")]
    public int Count300 { get; set; }

    [JsonProperty("count_100")]
    public int Count100 { get; set; }

    [JsonProperty("count_50")]
    public int Count50 { get; set; }

    [JsonProperty("count_miss")]
    public int CountMiss { get; set; }

    // [JsonProperty("level")]
    // public UserStatisticsLevelJsonModel Level { get; set; }

    [JsonProperty("global_rank")]
    public int? GlobalRank { get; set; }

    [JsonProperty("global_rank_exp")]
    public int? GlobalRankExp { get; set; }

    [JsonProperty("pp")]
    public double Pp { get; set; }

    [JsonProperty("pp_exp")]
    public double? PpExp { get; set; }

    [JsonProperty("ranked_score")]
    public long RankedScore { get; set; }

    [JsonProperty("hit_accuracy")]
    public double HitAccuracy { get; set; }

    [JsonProperty("play_count")]
    public int PlayCount { get; set; }

    [JsonProperty("play_time")]
    public int PlayTime { get; set; }

    [JsonProperty("total_score")]
    public long TotalScore { get; set; }

    [JsonProperty("total_hits")]
    public int TotalHits { get; set; }

    [JsonProperty("maximum_combo")]
    public int MaxCombo { get; set; }

    [JsonProperty("replays_watched_by_others")]
    public int ReplaysWatched { get; set; }

    [JsonProperty("is_ranked")]
    public bool IsRanked { get; set; }

    [JsonProperty("grade_counts")]
    public IDictionary<ScoreGrade, int> GradeCounts { get; set; } = new Dictionary<ScoreGrade, int>();

    [JsonProperty("country_rank")]
    public int? CountryRank { get; set; }

    [JsonProperty("variants")]
    public UserStatisticsVariantJsonModel[] Variants { get; set; } = [];
}
