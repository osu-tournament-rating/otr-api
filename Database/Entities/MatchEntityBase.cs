using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Database.Enums;

namespace Database.Entities;

/// <summary>
/// Base entity for matches
/// </summary>
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public abstract class MatchEntityBase : UpdateableEntityBase
{
    /// <summary>
    /// osu! id
    /// </summary>
    /// <example>https://osu.ppy.sh/community/matches/[113475484]</example>
    [Column("match_id")]
    public long MatchId { get; set; }

    /// <summary>
    /// Name of the lobby the match was played in
    /// </summary>
    /// <example>5WC2024: (France) vs (Germany)</example>
    [MaxLength(512)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp for the beginning of the match
    /// </summary>
    [Column("start_time")]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Timestamp for the end of the match
    /// </summary>
    [Column("end_time")]
    public DateTime EndTime { get; set; }

    // TODO: Data worker refactor
    /// <summary>
    /// The verification status of the match
    /// </summary>
    [Column("verification_status")]
    public Old_MatchVerificationStatus? VerificationStatus { get; set; }

    // TODO: Data worker refactor
    /// <summary>
    /// Indicates to background workers that the match needs to be processed
    /// </summary>
    [Column("needs_auto_check")]
    public bool? NeedsAutoCheck { get; set; }

    // TODO: Data worker refactor
    /// <summary>
    /// Indicates to background workers that the match needs to be processed with the osu! api
    /// </summary>
    [Column("is_api_processed")]
    public bool? IsApiProcessed { get; set; }

    /// <summary>
    /// Id of the <see cref="Tournament"/> the match was played in
    /// </summary>
    [Column("tournament_id")]
    public int TournamentId { get; set; }

    /// <summary>
    /// Id of the <see cref="User"/> that submitted the match
    /// </summary>
    [Column("submitted_by_user_id")]
    public int? SubmittedByUserId { get; set; }

    /// <summary>
    /// Id of the <see cref="User"/> that verified the match
    /// </summary>
    [Column("verified_by_user_id")]
    public int? VerifiedByUserId { get; set; }
}
