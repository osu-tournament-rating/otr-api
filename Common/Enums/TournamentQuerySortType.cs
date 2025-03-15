namespace Common.Enums;

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
    /// Sort by end date
    /// </summary>
    EndTime,

    /// <summary>
    /// Sort by name
    /// </summary>
    SearchQueryRelevance,

    /// <summary>
    /// Sort by created date
    /// </summary>
    Created,

    /// <summary>
    /// Sort by team lobby size
    /// </summary>
    TeamLobbySize
}
