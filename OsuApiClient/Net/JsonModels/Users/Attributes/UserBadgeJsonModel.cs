using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.Users.Attributes;

/// <summary>
/// No description
/// </summary>
/// <copyright>
/// ppy 2024 https://osu.ppy.sh/docs/index.html#user-userbadge
/// Last accessed May 2024
/// </copyright>
public class UserBadgeJsonModel : JsonModelBase
{
    [JsonProperty("awarded_at")]
    public DateTimeOffset AwardedAt { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; } = null!;

    [JsonProperty("image_url")]
    public string ImageUrl { get; set; } = null!;

    [JsonProperty("image@2x_url")]
    public string Image2XUrl { get; set; } = null!;

    [JsonProperty("url")]
    public string Url { get; set; } = null!;
}
