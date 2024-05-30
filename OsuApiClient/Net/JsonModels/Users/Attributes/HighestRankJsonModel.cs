using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.Users.Attributes;

/// <summary>
/// No description
/// </summary>
/// <copyright>
/// ppy 2024 https://osu.ppy.sh/docs/index.html#user-rankhighest
/// Last accessed May 2024
/// </copyright>
[SuppressMessage("ReSharper", "CommentTypo")]
public class HighestRankJsonModel : JsonModelBase
{
    [JsonProperty("rank")]
    public int Rank { get; set; }

    [JsonProperty("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }
}
