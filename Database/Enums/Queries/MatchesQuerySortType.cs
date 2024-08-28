namespace Database.Enums.Queries;

/// <summary>
/// Denotes which property a query for <see cref="Database.Entities.Matches"/> will be sorted by
/// </summary>
public enum MatchesQuerySortType
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
    /// Sort by start start time
    /// </summary>
    StartTime,

    /// <summary>
    /// Sort by end time
    /// </summary>
    EndTime
}
