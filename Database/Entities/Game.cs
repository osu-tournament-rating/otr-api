using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Database.Entities.Processor;
using Database.Enums;

namespace Database.Entities;

/// <summary>
/// A game played in a <see cref="Match"/>
/// </summary>
[Table("games")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")]
public class Game : UpdateableEntityBase
{
    /// <summary>
    /// osu! id
    /// </summary>
    [Column("game_id")]
    public long GameId { get; set; }

    /// <summary>
    /// The <see cref="Enums.Ruleset"/> the game was played in
    /// </summary>
    [Column("ruleset")]
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// The <see cref="Enums.ScoringType"/> used
    /// </summary>
    [Column("scoring_type")]
    public ScoringType ScoringType { get; set; }

    /// <summary>
    /// The <see cref="Enums.TeamType"/> used
    /// </summary>
    [Column("team_type")]
    public TeamType TeamType { get; set; }

    /// <summary>
    /// The enabled <see cref="Enums.Mods"/>
    /// </summary>
    [Column("mods")]
    public Mods Mods { get; set; }

    /// <summary>
    /// Star rating of the played beatmap after applying mods
    /// </summary>
    [Column("post_mod_sr")]
    public double PostModSr { get; set; }

    // TODO: Data worker refactor
    /// <summary>
    /// The verification status of the game
    /// </summary>
    [Column("verification_status")]
    public Old_GameVerificationStatus? VerificationStatus { get; set; }

    // TODO: Data worker refactor
    /// <summary>
    /// The reason the game was rejected from verification
    /// </summary>
    [Column("rejection_reason")]
    public Old_GameRejectionReason? RejectionReason { get; set; }

    /// <summary>
    /// Timestamp for the beginning of the game
    /// </summary>
    [Column("start_time")]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Timestamp for the end of the game
    /// </summary>
    [Column("end_time")]
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Id of the <see cref="Entities.Match"/> that the game was played in
    /// </summary>
    [Column("match_id")]
    public int MatchId { get; set; }

    /// <summary>
    /// The <see cref="Entities.Match"/> the game was played in
    /// </summary>
    public Match Match { get; set; } = null!;

    /// <summary>
    /// Id of the <see cref="Entities.Beatmap"/> played during the game
    /// </summary>
    [Column("beatmap_id")]
    public int? BeatmapId { get; set; }

    /// <summary>
    /// The <see cref="Entities.Beatmap"/> played during the game
    /// </summary>
    public Beatmap? Beatmap { get; set; }

    /// <summary>
    /// The win record for the game
    /// </summary>
    public GameWinRecord? WinRecord { get; set; }

    /// <summary>
    /// All match scores for the game
    /// </summary>
    public ICollection<MatchScore> MatchScores { get; set; } = new List<MatchScore>();
}
