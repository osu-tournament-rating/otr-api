using AutoMapper;
using OsuApiClient.Net.JsonModels.Osu.Beatmaps;

namespace OsuApiClient.Domain.Osu.Beatmaps;

/// <summary>
/// Collection of beatmaps
/// </summary>
[AutoMap(typeof(BeatmapCollectionJsonModel))]
public class BeatmapCollection : IModel
{
    /// <summary>
    /// Beatmaps
    /// </summary>
    public IEnumerable<BeatmapExtended> Beatmaps { get; init; } = [];
}
