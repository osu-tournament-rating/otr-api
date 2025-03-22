using System.Diagnostics.CodeAnalysis;
using Common.Enums;
using OsuApiClient.Domain.Osu.Multiplayer;

namespace OsuApiClient.Domain.Osu.Beatmaps;

/// <summary>
/// Represents a beatmap played in a <see cref="MultiplayerGame"/>
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class Beatmap : IModel
{
    /// <summary>
    /// Beatmap id
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Id of the beatmapset the beatmap is part of
    /// </summary>
    public long BeatmapsetId { get; init; }

    /// <summary>
    /// Star rating
    /// </summary>
    public double StarRating { get; init; }

    /// <summary>
    /// The <see cref="Common.Enums.Ruleset"/> this beatmap is playable on
    /// </summary>
    public Ruleset Ruleset { get; init; }

    /// <summary>
    /// Ranking status as a string
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Total length
    /// </summary>
    public long TotalLength { get; init; }

    /// <summary>
    /// Id of the beatmap submitter
    /// </summary>
    public long UserId { get; init; }

    /// <summary>
    /// Difficulty name
    /// </summary>
    public string DifficultyName { get; init; } = string.Empty;

    /// <summary>
    /// The <see cref="Beatmaps.Beatmapset"/> the beatmap is part of
    /// </summary>
    public Beatmapset? Beatmapset { get; init; }
}
