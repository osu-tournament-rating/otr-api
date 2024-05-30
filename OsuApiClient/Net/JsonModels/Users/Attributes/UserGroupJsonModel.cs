using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using OsuApiClient.Net.JsonModels.Site;

namespace OsuApiClient.Net.JsonModels.Users.Attributes;

/// <summary>
/// Describes a Group membership of a User.
/// It contains all of the attributes of the Group, in addition to what is listed here.
/// </summary>
/// <copyright>
/// ppy 2024 https://osu.ppy.sh/docs/index.html#usergroup
/// Last accessed May 2024
/// </copyright>
[SuppressMessage("ReSharper", "CommentTypo")]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class UserGroupJsonModel : GroupJsonModel
{
    [JsonProperty("playmodes")]
    public string[]? PlayModes { get; set; }
}
