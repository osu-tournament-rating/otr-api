namespace Database.Utilities;

/// <summary>
/// The property will be excluded from audits created of the declaring type
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class AuditIgnoreAttribute : Attribute;
