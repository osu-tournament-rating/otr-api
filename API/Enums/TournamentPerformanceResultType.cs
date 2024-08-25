using API.DTOs;

namespace API.Enums;

/// <summary>
/// Denotes the manner in which a list of <see cref="PlayerTournamentMatchCostDTO"/>s (aka performances) are sorted
/// </summary>
public enum TournamentPerformanceResultType
{
    /// <summary>
    /// Performances sorted by recent date
    /// </summary>
    Recent,

    /// <summary>
    /// Performances sorted by highest average match cost
    /// </summary>
    Best,

    /// <summary>
    /// Performances sorted by lowest average match cost
    /// </summary>
    Worst
}
