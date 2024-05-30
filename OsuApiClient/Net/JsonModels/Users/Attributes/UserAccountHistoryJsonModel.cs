using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.Users.Attributes;

/// <summary>
/// No description
/// </summary>
/// <copyright>
/// ppy 2024 https://osu.ppy.sh/docs/index.html#user-useraccounthistory
/// Last accessed May 2024
/// </copyright>
[SuppressMessage("ReSharper", "CommentTypo")]
public class UserAccountHistoryJsonModel : JsonModelBase
{
    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("length")]
    public int Length { get; set; }

    [JsonProperty("permanent")]
    public bool Permanent { get; set; }

    [JsonProperty("timestamp")]
    public DateTimeOffset Timestamp { get; set; }

    [JsonProperty("type")]
    public string? Type { get; set; }
}
