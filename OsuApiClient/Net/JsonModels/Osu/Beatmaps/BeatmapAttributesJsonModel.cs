using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.Osu.Beatmaps;

/// <summary>
/// Represents beatmap difficulty attributes
/// </summary>
/// <copyright>
/// ppy 2025 https://osu.ppy.sh/docs/index.html#beatmapdifficultyattributes
/// Last accessed January 2025
/// </copyright>
[SuppressMessage("ReSharper", "CommentTypo")]
public class BeatmapAttributesJsonModel : JsonModelBase
{
    [JsonProperty("max_combo")]
    public int MaxCombo { get; set; }

    [JsonProperty("star_rating")]
    public double StarRating { get; set; }
}
