using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.Beatmaps;

/// <summary>
/// No description
/// </summary>
/// <remarks>Undocumented</remarks>
/// <copyright>
/// ppy 2024
/// Last accessed June 2024
/// </copyright>
public class BeatmapCoversJsonModel
{
    [JsonProperty("cover")]
    public string Cover { get; set; } = null!;

    [JsonProperty("cover@2x")]
    public string Cover2X { get; set; } = null!;

    [JsonProperty("card")]
    public string Card { get; set; } = null!;

    [JsonProperty("card@2x")]
    public string Card2X { get; set; } = null!;

    [JsonProperty("list")]
    public string List { get; set; } = null!;

    [JsonProperty("list@2x")]
    public string List2X { get; set; } = null!;

    [JsonProperty("slimcover")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public string SlimCover { get; set; } = null!;

    [JsonProperty("slimcover@2x")]
    public string SlimCover2X { get; set; } = null!;
}
