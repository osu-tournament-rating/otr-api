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
    /// Sort by submission date
    /// </summary>
    SubmissionDate,

    /// <summary>
    /// Sort by lobby size
    /// </summary>
    LobbySize
}
