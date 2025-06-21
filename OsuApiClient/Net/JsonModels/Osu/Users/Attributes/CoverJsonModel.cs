using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.Osu.Users.Attributes;

/// <summary>
/// No description
/// </summary>
/// <remarks>Undocumented</remarks>
/// <copyright>
/// ppy 2024
/// Last accessed May 2024
/// </copyright>
public class CoverJsonModel
{
    [JsonProperty("id")]
    public int? Id { get; set; }

    [JsonProperty("url")]
    public string? Url { get; set; }

    [JsonProperty("custom_url")]
    public string? CustomUrl { get; set; }
}
