namespace Database.Entities;

/// <summary>
/// Represents a change to a property for an audit
/// </summary>
public class AuditChangelogEntry : ICloneable
{
    /// <summary>
    /// Original value of the property
    /// </summary>
    public object OriginalValue { get; init; } = null!;

    /// <summary>
    /// New value for the property
    /// </summary>
    public object NewValue { get; init; } = null!;

    public object Clone() => new AuditChangelogEntry { OriginalValue = OriginalValue, NewValue = NewValue };
}
