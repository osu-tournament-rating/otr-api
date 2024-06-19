using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Database.Enums;
using OsuApiClient.Net.Deserialization.ValueConverters;
using OsuApiClient.Net.JsonModels.Multiplayer;

namespace OsuApiClient.Domain.Multiplayer;

/// <summary>
/// Represents a beatmap played in a <see cref="MultiplayerGame"/>
/// </summary>
[AutoMap(typeof(GameBeatmapJsonModel))]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class GameBeatmap : IModel
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
    [SourceMember(nameof(GameBeatmapJsonModel.DifficultyRating))]
    public double StarRating { get; init; }

    /// <summary>
    /// The <see cref="Database.Enums.Ruleset"/> this beatmap is playable on
    /// </summary>
    [ValueConverter(typeof(RulesetConverter))]
    [SourceMember(nameof(GameBeatmapJsonModel.Mode))]
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
    [SourceMember(nameof(GameBeatmapJsonModel.Version))]
    public string DifficultyName { get; init; } = null!;

    /// <summary>
    /// The <see cref="GameBeatmapset"/> the beatmap is part of
    /// </summary>
    public GameBeatmapset? Beatmapset { get; init; }
}
