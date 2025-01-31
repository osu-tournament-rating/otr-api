using AutoMapper;
using OsuApiClient.Net.JsonModels.Osu.Beatmaps;

namespace OsuApiClient.Domain.Osu.Beatmaps;

/// <summary>
/// Represents beatmap difficulty attributes
/// </summary>
[AutoMap(typeof(BeatmapAttributesJsonModel))]
public class BeatmapAttributes : IModel
{
    /// <summary>
    /// Max possible combo
    /// </summary>
    public int MaxCombo { get; set; }

    /// <summary>
    /// Star rating
    /// </summary>
    public double StarRating { get; set; }
}
