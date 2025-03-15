using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Common.Enums;
using OsuApiClient.Domain.Osu.Beatmaps;
using OsuApiClient.Net.Deserialization.ValueConverters;
using OsuApiClient.Net.JsonModels.Osu.Multiplayer;

namespace OsuApiClient.Domain.Osu.Multiplayer;

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
    /// The <see cref="Common.Enums.Ruleset"/> the game was played in
    /// </summary>
    [SourceMember(nameof(MultiplayerGameJsonModel.ModeInt))]
    public Ruleset Ruleset { get; init; }

    /// <summary>
    /// The <see cref="Common.Enums.ScoringType"/> used for the game
    /// </summary>
    [ValueConverter(typeof(ScoringTypeConverter))]
    public ScoringType ScoringType { get; init; }

    /// <summary>
    /// The <see cref="Common.Enums.TeamType"/> used for the game
    /// </summary>
    [ValueConverter(typeof(TeamTypeConverter))]
    public TeamType TeamType { get; init; }

    /// <summary>
    /// Any <see cref="Common.Enums.Mods"/> forced on the lobby
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
