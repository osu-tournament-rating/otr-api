using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Database.Enums.VerificationEnums;

namespace Database.Entities;

/// <summary>
/// TBD
/// </summary>
[Table("pending_tournaments")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class PendingTournament : TournamentEntityBase
{
    /// <summary>
    /// The current API processing status of the tournament
    /// </summary>
    [Column("processing_status")]
    public TournamentProcessingStatus ProcessingStatus { get; set; }

    /// <summary>
    /// The current verification status of the tournament
    /// </summary>
    [Column("verification_status")]
    public VerificationStatus VerificationStatus { get; set; }

    /// <summary>
    /// The current rejection reason of the tournament
    /// </summary>
    [Column("rejection_reason")]
    public TournamentRejectionReason RejectionReason { get; set; }
}
