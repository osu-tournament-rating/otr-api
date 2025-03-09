namespace Common.Enums.Enums.Queries;

/// <summary>
/// Denotes which property a query for <see cref="Database.Entities.Matches"/> will be sorted by
/// </summary>
public enum MatchQuerySortType
{
    /// <summary>
    /// Sort by primary key
    /// </summary>
    Id,

    /// <summary>
    /// Sort by osu! id
    /// </summary>
    OsuId,

    /// <summary>
    /// Sort by start time
    /// </summary>
    StartTime,

    /// <summary>
    /// Sort by end time
    /// </summary>
    EndTime,

    /// <summary>
    /// Sort by creation date
    /// </summary>
    Created
}
