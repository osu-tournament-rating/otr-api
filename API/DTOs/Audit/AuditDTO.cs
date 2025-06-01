using Common.Enums;

namespace API.DTOs.Audit;

public class AuditDTO
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public AuditActionType ActionType { get; set; }
    public AuditEntityType EntityType { get; set; }
    public DateTime Timestamp { get; set; }
    public int EntityId { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
}
