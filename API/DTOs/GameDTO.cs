using System.Diagnostics.CodeAnalysis;
using API.Osu.Enums;

namespace API.DTOs;

/// <summary>
/// Represents a single game (osu! beatmap) played in a match
/// </summary>
public class GameDTO
{
    /// <summary>
    /// Id of the game
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The ruleset for the game
    /// </summary>
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// The scoring type used for the game
    /// </summary>
    public ScoringType ScoringType { get; set; }

    /// <summary>
    /// The team type used for the game
    /// </summary>
    public TeamType TeamType { get; set; }

    /// <summary>
    /// The mods enabled for the game
    /// </summary>
    public Mods Mods { get; set; }

    /// <summary>
    /// osu! id of the game
    /// </summary>
    public long GameId { get; set; }

    /// <summary>
    /// Timestamp of the beginning of the game
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Timestamp of the end of the game
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// The beatmap the game was played on
    /// </summary>
    public BeatmapDTO? Beatmap { get; set; }

    /// <summary>
    /// All match scores for the game
    /// </summary>
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    public List<MatchScoreDTO> MatchScores { get; set; } = [];
}
