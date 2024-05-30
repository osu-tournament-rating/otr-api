using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.Users.Attributes;

/// <summary>
/// No description
/// </summary>
/// <copyright>
/// ppy 2024 https://osu.ppy.sh/docs/index.html#user-kudosu
/// Last accessed May 2024
/// </copyright>
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "CommentTypo")]
public class KudosuJsonModel
{
    [JsonProperty("available")]
    public int Available { get; set; }

    [JsonProperty("total")]
    public int Total { get; set; }
}
