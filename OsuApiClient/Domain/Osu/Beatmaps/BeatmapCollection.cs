using AutoMapper;
using JetBrains.Annotations;
using OsuApiClient.Net.JsonModels.Osu.Beatmaps;

namespace OsuApiClient.Domain.Osu.Beatmaps;

/// <summary>
/// Collection of beatmaps
/// </summary>
[AutoMap(typeof(BeatmapCollectionJsonModel))]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class BeatmapCollection : IModel
{
    /// <summary>
    /// Beatmaps
    /// </summary>
    public IEnumerable<BeatmapExtended> Beatmaps { get; init; } = [];
}
