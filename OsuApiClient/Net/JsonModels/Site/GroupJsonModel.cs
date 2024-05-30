using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.Site;

/// <summary>
/// This object is not returned by any endpoints yet. It is here only as a reference for <see cref="UserGroupJsonModel"/>
/// </summary>
/// <copyright>
/// ppy 2024 https://osu.ppy.sh/docs/index.html#group
/// Last accessed May 2024
/// </copyright>
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class GroupJsonModel : JsonModelBase
{
    [JsonProperty("colour")]
    public string? Color { get; set; }

    [JsonProperty("has_listing")]
    public bool HasListing { get; set; }

    [JsonProperty("has_playmodes")]
    public bool HasPlayModes { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("identifier")]
    public string Identifier { get; set; } = null!;

    [JsonProperty("is_probationary")]
    public bool IsProbationary { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("short_name")]
    public string ShortName { get; set; } = null!;
}
