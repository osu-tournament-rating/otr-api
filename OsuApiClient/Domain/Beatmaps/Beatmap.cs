using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Database.Enums;
using OsuApiClient.Domain.Multiplayer;
using OsuApiClient.Net.Deserialization.ValueConverters;
using OsuApiClient.Net.JsonModels.Beatmaps;

namespace OsuApiClient.Domain.Beatmaps;

/// <summary>
/// Represents a beatmap played in a <see cref="MultiplayerGame"/>
/// </summary>
[AutoMap(typeof(BeatmapJsonModel))]
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
    [SourceMember(nameof(BeatmapJsonModel.DifficultyRating))]
    public double StarRating { get; init; }

    /// <summary>
    /// The <see cref="Database.Enums.Ruleset"/> this beatmap is playable on
    /// </summary>
    [ValueConverter(typeof(RulesetConverter))]
    [SourceMember(nameof(BeatmapJsonModel.Mode))]
    public Ruleset Ruleset { get; init; }

    /// <summary>
    /// Ranking status
    /// </summary>
    public string Status { get; init; } = null!;

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
    [SourceMember(nameof(BeatmapJsonModel.Version))]
    public string DifficultyName { get; init; } = null!;

    /// <summary>
    /// The <see cref="Beatmaps.Beatmapset"/> the beatmap is part of
    /// </summary>
    public Beatmapset? Beatmapset { get; init; }
}
