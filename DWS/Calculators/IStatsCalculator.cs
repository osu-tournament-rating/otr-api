using Database.Entities;

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
    /// <returns>True if calculations completed successfully, false otherwise.</returns>
    bool CalculateAllStatistics(Tournament tournament);
}
