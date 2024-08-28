using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities;

/// <summary>
/// Tracks changes to <see cref="Game"/> entities
/// </summary>
[Table("game_audits")]
public class GameAudit : AuditEntityBase<Game, GameAudit>;
