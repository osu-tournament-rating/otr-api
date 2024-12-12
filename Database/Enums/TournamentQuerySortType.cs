namespace Database.Enums;

/// <summary>
/// Defines how to sort the results of fetching all tournaments
/// </summary>
public enum TournamentQuerySortType
{
    /// <summary>
    /// Sort by primary key
    /// </summary>
    Id,

    /// <summary>
    /// Sort by start date
    /// </summary>
    StartTime,

    /// <summary>
    /// Sort by start date
    /// </summary>
    EndTime,

    /// <summary>
    /// Sort by name
    /// </summary>
    Name,

    /// <summary>
    /// Sort by created date
    /// </summary>
    Created
}
