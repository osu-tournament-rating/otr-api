using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities;

/// <summary>
/// Tracks changes to <see cref="GameScore"/> entities
/// </summary>
[Table("game_score_audits")]
public class GameScoreAudit : AuditEntityBase<GameScore, GameScoreAudit>;
