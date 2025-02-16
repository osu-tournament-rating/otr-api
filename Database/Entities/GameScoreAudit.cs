using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities;

/// <summary>
/// Tracks changes to <see cref="GameScore"/> entities
/// </summary>
public class GameScoreAudit : AuditEntityBase<GameScore, GameScoreAudit>;
