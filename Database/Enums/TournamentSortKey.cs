namespace API.Enums;

/// <summary>
/// Defines how to sort the results of fetching all tournaments
/// </summary>
public enum TournamentSortKey
{
    /// <summary>
    /// Do not sort
    /// </summary>
    None,
    /// <summary>
    /// Sort by start date ascending
    /// </summary>
    StartTime,
    /// <summary>
    /// Sort by start date descending
    /// </summary>
    StartTimeDescending,
    /// <summary>
    /// Sort by name ascending
    /// </summary>
    Name,
    /// <summary>
    /// Sort by name descending
    /// </summary>
    NameDescending,
    /// <summary>
    /// Sort by created date ascending
    /// </summary>
    Created,
    /// <summary>
    /// Sort by created date descending
    /// </summary>
    CreatedDescending
}
