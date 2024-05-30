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
public class RankHistoryJsonModel : JsonModelBase
{
    [JsonProperty("mode")]
    public string Mode { get; set; } = null!;

    [JsonProperty("data")]
    public long[] Data { get; set; } = null!;
}
