namespace Database.Entities;

/// <summary>
/// Tracks changes to <see cref="Tournament"/> entities
/// </summary>
public class TournamentAudit : AuditEntityBase<Tournament, TournamentAudit>;
