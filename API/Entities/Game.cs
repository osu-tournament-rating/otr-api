using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using API.Entities.Interfaces;
using API.Enums;
using API.Osu.Enums;
using Microsoft.EntityFrameworkCore;
// ReSharper disable StringLiteralTypo

namespace API.Entities;

/// <summary>
/// Represents a single game (osu! map) played in a tournament match
/// </summary>
[Table("games")]
[Index("GameId", Name = "osugames_gameid", IsUnique = true)]
[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class Game : IUpdateableEntity
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// The id of the match the game was played in
    /// </summary>
    [Column("match_id")]
    public int MatchId { get; set; }

    /// <summary>
    /// The id of the beatmap the game was played on
    /// </summary>
    [Column("beatmap_id")]
    public int? BeatmapId { get; set; }

    /// <summary>
    /// The ruleset for the game
    /// </summary>
    [Column("ruleset")]
    public Ruleset Ruleset { get; set; }

    /// <summary>
    /// The scoring type used for the game
    /// </summary>
    [Column("scoring_type")]
    public ScoringType ScoringType { get; set; }

    /// <summary>
    /// The team type used for the game
    /// </summary>
    [Column("team_type")]
    public TeamType TeamType { get; set; }

    /// <summary>
    /// The mods enabled for the game
    /// </summary>
    [Column("mods")]
    public Mods Mods { get; set; }

    /// <summary>
    /// Star rating of the played beatmap after applying mods
    /// </summary>
    [Column("post_mod_sr")]
    public double PostModSr { get; set; }

    /// <summary>
    /// The osu! id for the game
    /// </summary>
    [Column("game_id")]
    public long GameId { get; set; }

    /// <summary>
    /// The verification status of the game
    /// </summary>
    [Column("verification_status")]
    public GameVerificationStatus? VerificationStatus { get; set; }

    /// <summary>
    /// The reason the game was rejected from verification
    /// </summary>
    [Column("rejection_reason")]
    public GameRejectionReason? RejectionReason { get; set; }

    /// <summary>
    /// Date the entity was created
    /// </summary>
    [Column("created", TypeName = "timestamp with time zone")]
    public DateTime Created { get; set; }

    /// <summary>
    /// Timestamp of the beginning of the game
    /// </summary>
    [Column("start_time", TypeName = "timestamp with time zone")]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Timestamp of the end of the game
    /// </summary>
    [Column("end_time", TypeName = "timestamp with time zone")]
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// Date of the last update to the entity
    /// </summary>
    [Column("updated", TypeName = "timestamp with time zone")]
    public DateTime? Updated { get; set; }

    /// <summary>
    /// The match the game was played in
    /// </summary>
    [ForeignKey("MatchId")]
    [InverseProperty("Games")]
    public virtual Match Match { get; set; } = null!;

    /// <summary>
    /// The beatmap the game was played on
    /// </summary>
    [ForeignKey("BeatmapId")]
    [InverseProperty("Games")]
    public virtual Beatmap? Beatmap { get; set; }

    /// <summary>
    /// All match scores for the game
    /// </summary>
    [InverseProperty("Game")]
    public virtual ICollection<MatchScore> MatchScores { get; set; } = new List<MatchScore>();

    /// <summary>
    /// The win record for the game
    /// </summary>
    [InverseProperty("Game")]
    public virtual GameWinRecord WinRecord { get; set; } = null!;
}
