using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities;

/// <summary>
/// Tracks changes to <see cref="Match"/> entities
/// </summary>
public class MatchAudit : AuditEntityBase<Match, MatchAudit>;
