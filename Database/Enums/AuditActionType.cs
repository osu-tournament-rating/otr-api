namespace Database.Enums;

/// <summary>
/// Denotes the type of action taken on an entity for an audit
/// </summary>
public enum AuditActionType
{
    /// <summary>
    /// The entity was created
    /// </summary>
    Created,

    /// <summary>
    /// The entity was updated
    /// </summary>
    Updated,

    /// <summary>
    /// The entity was deleted
    /// </summary>
    Deleted
}
