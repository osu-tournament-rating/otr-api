using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities;

/// <summary>
/// Tracks changes to <see cref="Match"/> entities
/// </summary>
[Table("match_audits")]
public class MatchAudit : AuditEntityBase<Match, MatchAudit>;
