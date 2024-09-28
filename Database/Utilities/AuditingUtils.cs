using Database.Entities.Interfaces;

namespace Database.Utilities;

/// <summary>
/// Helper methods for auditing changes in database entities
/// </summary>
public static class AuditingUtils
{
    /// <summary>
    /// Collection of property names that will not trigger an audit if changed
    /// </summary>
    public static readonly IEnumerable<string> BlacklistedPropNames = new[]
    {
        nameof(IUpdateableEntity.Id),
        nameof(IUpdateableEntity.Created),
        nameof(IUpdateableEntity.Updated),
        nameof(IProcessableEntity.LastProcessingDate)
    };
}
