using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using OsuApiClient.Net.JsonModels.Osu.Beatmaps;

namespace OsuApiClient.Domain.Osu.Beatmaps;

/// <summary>
/// Represents a collection of urls to the cover images for a beatmap
/// </summary>
[AutoMap(typeof(BeatmapsetCoversJsonModel))]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class BeatmapsetCovers
{
    /// <summary>
    /// Cover image url
    /// </summary>
    public string Cover { get; init; } = null!;

    /// <summary>
    /// Cover image url in 2x resolution
    /// </summary>
    public string Cover2X { get; init; } = null!;

    /// <summary>
    /// Card image url
    /// </summary>
    public string Card { get; init; } = null!;

    /// <summary>
    /// Card image url in 2x resolution
    /// </summary>
    public string Card2X { get; init; } = null!;

    /// <summary>
    /// List image url
    /// </summary>
    public string List { get; init; } = null!;

    /// <summary>
    /// List image url in 2x resolution
    /// </summary>
    public string List2X { get; init; } = null!;

    /// <summary>
    /// Slim cover image url
    /// </summary>
    public string SlimCover { get; init; } = null!;

    /// <summary>
    /// Slim cover image url in 2x resolution
    /// </summary>
    public string SlimCover2X { get; init; } = null!;
}
