using System.ComponentModel.DataAnnotations;
using Common.Enums;

namespace API.DTOs.Audit;

/// <summary>
/// Represents an audit record tracking changes made to entities in the system
/// </summary>
public class AuditDTO
{
    /// <summary>
    /// Primary key of the audit record
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Id of the user who performed the action, if available
    /// </summary>
    /// <remarks>
    /// Will be null for system-generated actions or actions performed by anonymous users
    /// </remarks>
    public int? UserId { get; set; }

    /// <summary>
    /// The type of action that was performed on the entity
    /// </summary>
    [EnumDataType(typeof(AuditActionType))]
    public AuditActionType ActionType { get; set; }

    /// <summary>
    /// The type of entity that was modified
    /// </summary>
    [EnumDataType(typeof(AuditEntityType))]
    public AuditEntityType EntityType { get; set; }

    /// <summary>
    /// Timestamp when the action was performed
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Id of the entity that was modified
    /// </summary>
    /// <remarks>
    /// This is a locked copy of the entity's primary key that persists even if the original entity is deleted
    /// </remarks>
    public int EntityId { get; set; }

    /// <summary>
    /// JSON object containing all field changes made to the entity.
    /// Format: { "FieldName": { "NewValue": value, "OriginalValue": value }, ... }
    /// </summary>
    public string Changes { get; set; } = string.Empty;
}
