using Database.Entities;
using DWS.Models;

namespace DWS.Calculators;

/// <summary>
/// Base interface for functional statistics calculators.
/// </summary>
public interface IStatsCalculator
{
    /// <summary>
    /// Performs in-memory statistics calculations on tournament data.
    /// </summary>
    /// <param name="tournament">The fully loaded tournament with all related data.</param>
    /// <returns>Result containing success status and statistics counts.</returns>
    StatsCalculationResult CalculateAllStatistics(Tournament tournament);
}
