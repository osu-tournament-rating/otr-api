namespace Database.Enums;

/// <summary>
/// Represents different behaviors for query filters
/// </summary>
[Flags]
public enum QueryFilterType
{
    /// <summary>
    /// Does not filter the query
    /// </summary>
    None = 0,

    /// <summary>
    /// Filters the query for those with a <see cref="VerificationStatus"/> of <see cref="VerificationStatus.Verified"/>
    /// </summary>
    Verified = 1 << 0,

    /// <summary>
    /// Filters the query for those that have completed processing
    /// </summary>
    ProcessingCompleted = 1 << 1
}
