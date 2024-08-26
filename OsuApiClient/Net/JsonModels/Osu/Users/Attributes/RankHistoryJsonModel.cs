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
public class RankHistoryJsonModel : JsonModelBase
{
    [JsonProperty("ruleset")]
    public string Mode { get; set; } = null!;

    [JsonProperty("data")]
    public long[] Data { get; set; } = null!;
}
