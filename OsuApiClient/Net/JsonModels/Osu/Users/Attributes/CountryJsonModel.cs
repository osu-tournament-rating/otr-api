using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.Osu.Users.Attributes;

/// <summary>
/// Represents a user's country
/// </summary>
/// <remarks>Undocumented</remarks>
/// <copyright>
/// ppy 2024
/// Last accessed May 2024
/// </copyright>
public class CountryJsonModel
{
    [JsonProperty("code")]
    public string? Code { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }
}
