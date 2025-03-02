namespace Database.Entities;

/// <summary>
/// Tracks changes to <see cref="Game"/> entities
/// </summary>
public class GameAudit : AuditEntityBase<Game, GameAudit>;
