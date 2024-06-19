using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using OsuApiClient.Net.JsonModels.Beatmaps;

namespace OsuApiClient.Domain.Beatmaps;

/// <summary>
/// Represents a beatmaps hype
/// </summary>
[AutoMap(typeof(BeatmapsetHypeJsonModel))]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class BeatmapsetHype : IModel
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
