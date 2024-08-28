using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.Osu.Users.Attributes;

/// <summary>
/// No description
/// </summary>
/// <remarks>Undocumented</remarks>
/// <copyright>
/// ppy 2024
/// Last accessed June 2024
/// </copyright>
public class UserStatisticsVariantJsonModel : JsonModelBase
{
    [JsonProperty("ruleset")]
    public string Mode { get; set; } = null!;

    [JsonProperty("variant")]
    public string Variant { get; set; } = null!;

    [JsonProperty("country_rank")]
    public int? CountryRank { get; set; }

    [JsonProperty("global_rank")]
    public int? GlobalRank { get; set; }

    [JsonProperty("pp")]
    public double Pp { get; set; }
}
