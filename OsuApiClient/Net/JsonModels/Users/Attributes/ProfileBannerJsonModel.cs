using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.Users.Attributes;

/// <summary>
/// No description
/// </summary>
/// <copyright>
/// ppy 2024 https://osu.ppy.sh/docs/index.html#user-profilebanner
/// Last accessed May 2024
/// </copyright>
public class ProfileBannerJsonModel : JsonModelBase
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("tournament_id")]
    public int TournamentId { get; set; }

    [JsonProperty("image")]
    public string? Image { get; set; }

    [JsonProperty("image@2x")]
    public string? Image2X { get; set; }
}
