namespace Database.Queries.Enums;

/// <summary>
/// Denotes which property a query for <see cref="Entities.Tournament"/>s will be sorted by
/// </summary>
public enum TournamentsQuerySortType
{
    /// <summary>
    /// Sort by primary key
    /// </summary>
    Id,

    /// <summary>
    /// Sort by the start date
    /// </summary>
    /// <remarks>
    /// The "end date" represents the <see cref="Entities.Match.StartTime"/> of the
    /// first <see cref="Entities.Match"/> played in the <see cref="Entities.Tournament"/>
    /// </remarks>
    StartTime,

    /// <summary>
    /// Sort by the end date
    /// </summary>
    /// <remarks>
    /// The "end date" represents the <see cref="Entities.Match.EndTime"/> of the
    /// last <see cref="Entities.Match"/> played in the <see cref="Entities.Tournament"/>
    /// </remarks>
    EndTime
}
