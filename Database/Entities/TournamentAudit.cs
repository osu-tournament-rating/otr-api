using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities;

/// <summary>
/// Tracks changes to <see cref="Tournament"/> entities
/// </summary>
[Table("tournament_audits")]
public class TournamentAudit : AuditEntityBase<Tournament, TournamentAudit>;
