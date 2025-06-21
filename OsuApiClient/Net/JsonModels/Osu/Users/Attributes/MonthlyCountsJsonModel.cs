using System.Diagnostics.CodeAnalysis;
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
[SuppressMessage("ReSharper", "CommentTypo")]
public class MonthlyCountsJsonModel
{
    [JsonProperty("start_date")]
    public DateTime StartDate { get; set; }

    [JsonProperty("count")]
    public int Count { get; set; }
}
