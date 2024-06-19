using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using OsuApiClient.Net.JsonModels.Beatmaps;

namespace OsuApiClient.Domain.Beatmaps;

/// <summary>
/// Represents a beatmaps hype
/// </summary>
[AutoMap(typeof(BeatmapHypeJsonModel))]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class BeatmapHype : IModel
{
    /// <summary>
    /// Current hype
    /// </summary>
    public int CurrentHype { get; init; }

    /// <summary>
    /// Required hype for consideration
    /// </summary>
    public int RequiredHype { get; init; }
}
