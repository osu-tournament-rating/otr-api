using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.Osu.Beatmaps;

/// <summary>
/// No description
/// </summary>
/// <remarks>Undocumented</remarks>
/// <copyright>
/// ppy 2025
/// Last accessed January 2025
/// </copyright>
public class BeatmapCollectionJsonModel : JsonModelBase
{
    [JsonProperty("beatmaps")]
    public IEnumerable<BeatmapExtendedJsonModel> Beatmaps { get; init; } = [];
}
