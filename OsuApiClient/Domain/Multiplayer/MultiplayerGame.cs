using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Database.Enums;
using OsuApiClient.Domain.Beatmaps;
using OsuApiClient.Net.Deserialization.ValueConverters;
using OsuApiClient.Net.JsonModels.Multiplayer;

namespace OsuApiClient.Domain.Multiplayer;

/// <summary>
/// Represents a game played in a <see cref="MultiplayerMatch"/>
/// </summary>
[AutoMap(typeof(MultiplayerGameJsonModel))]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class MultiplayerGame : IModel
{
    /// <summary>
    /// Game id
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Beatmap id
    /// </summary>
    public long BeatmapId { get; init; }

    /// <summary>
    /// Timestamp for the start time of the game
    /// </summary>
    public DateTime StartTime { get; init; }

    /// <summary>
    /// Timestamp for the end time of the game
    /// </summary>
    public DateTime? EndTime { get; init; }

    /// <summary>
    /// The <see cref="Database.Enums.Ruleset"/> the game was played in
    /// </summary>
    [ValueConverter(typeof(RulesetConverter))]
    [SourceMember(nameof(MultiplayerGameJsonModel.Mode))]
    public Ruleset Ruleset { get; init; }

    /// <summary>
    /// The <see cref="Database.Enums.ScoringType"/> used for the game
    /// </summary>
    [ValueConverter(typeof(ScoringTypeConverter))]
    public ScoringType ScoringType { get; init; }

    /// <summary>
    /// The <see cref="Database.Enums.TeamType"/> used for the game
    /// </summary>
    [ValueConverter(typeof(TeamTypeConverter))]
    public TeamType TeamType { get; init; }

    /// <summary>
    /// Any <see cref="Database.Enums.Mods"/> forced on the lobby
    /// </summary>
    [ValueConverter(typeof(ModsConverter))]
    public Mods Mods { get; init; }

    /// <summary>
    /// The played <see cref="Beatmaps.Beatmap"/>
    /// </summary>
    public Beatmap? Beatmap { get; init; }

    /// <summary>
    /// A list of all scores set
    /// </summary>
    public GameScore[] Scores { get; init; } = [];
}
