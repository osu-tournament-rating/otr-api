namespace Database.Utilities;

/// <summary>
/// Attribute to mark properties that should be ignored during audit operations
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class AuditIgnoreAttribute : Attribute;
