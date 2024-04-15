using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using API.Enums;

namespace API.Entities;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class MatchEntityBase
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// The osu! match id
    /// </summary>
    [Column("match_id")]
    public long MatchId { get; set; }

    /// <summary>
    /// Name of the lobby the match was played in
    /// </summary>
    [Column("name")]
    [MaxLength(512)]
    public string? Name { get; set; }

    /// <summary>
    /// Timestamp of the beginning of the match
    /// </summary>
    [Column("start_time", TypeName = "timestamp with time zone")]
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// Timestamp of the end of the match
    /// </summary>
    [Column("end_time", TypeName = "timestamp with time zone")]
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// Any additional information included when <see cref="VerificationStatus"/> changes
    /// </summary>
    [Column("verification_info")]
    [MaxLength(512)]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public string? VerificationInfo { get; set; }

    /// <summary>
    /// The type of source that verified the match
    /// </summary>
    [Column("verification_source")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public MatchVerificationSource? VerificationSource { get; set; }

    /// <summary>
    /// The verification status of the match
    /// </summary>
    [Column("verification_status")]
    public MatchVerificationStatus? VerificationStatus { get; set; }

    /// <summary>
    /// The id of the user that verified the match
    /// </summary>
    [Column("verified_by_user")]
    public int? VerifierUserId { get; set; }

    /// <summary>
    /// The id of the tournament the match was played in
    /// </summary>
    [Column("tournament_id")]
    public int TournamentId { get; set; }

    /// <summary>
    /// Indicates to background workers that the match needs to be processed
    /// </summary>
    [Column("needs_auto_check")]
    public bool? NeedsAutoCheck { get; set; }

    /// <summary>
    /// Indicates to background workers that the match needs to be processed with the osu! api
    /// </summary>
    [Column("is_api_processed")]
    public bool? IsApiProcessed { get; set; }

    /// <summary>
    /// The id of the user that submitted the match
    /// </summary>
    [Column("submitted_by_user")]
    public int? SubmitterUserId { get; set; }
}
