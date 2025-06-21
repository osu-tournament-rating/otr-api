using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.Osu.Users;

/// <summary>
/// Represents a user
/// </summary>
/// <copyright>
/// ppy 2024 https://osu.ppy.sh/docs/index.html#user
/// Last accessed May 2024
/// </copyright>
public class UserJsonModel
{
    [JsonProperty("avatar_url")]
    public string AvatarUrl { get; set; } = string.Empty;

    [JsonProperty("country_code")]
    public string CountryCode { get; set; } = string.Empty;

    [JsonProperty("default_group")]
    public string? DefaultGroup { get; set; }

    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("is_active")]
    public bool IsActive { get; set; }

    [JsonProperty("is_bot")]
    public bool IsBot { get; set; }

    [JsonProperty("is_deleted")]
    public bool IsDeleted { get; set; }

    [JsonProperty("is_online")]
    public bool IsOnline { get; set; }

    [JsonProperty("is_supporter")]
    public bool IsSupporter { get; set; }

    [JsonProperty("last_visit")]
    public DateTimeOffset? LastVisit { get; set; }

    [JsonProperty("pm_friends_only")]
    public bool PmFriendsOnly { get; set; }

    [JsonProperty("profile_colour")]
    public string? ProfileColor { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; } = string.Empty;
}
