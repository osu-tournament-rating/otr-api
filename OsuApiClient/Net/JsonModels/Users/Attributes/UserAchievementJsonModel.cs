using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.Users.Attributes;

/// <summary>
/// No description
/// </summary>
/// <remarks>Undocumented</remarks>
/// <copyright>
/// ppy 2024
/// Last accessed May 2024
/// </copyright>
public class UserAchievementJsonModel : JsonModelBase
{
    [JsonProperty("achieved_at")]
    public DateTimeOffset AchievedAt { get; set; }

    [JsonProperty("achievement_id")]
    public int AchievementId { get; set; }
}
