namespace DWS.Models;

/// <summary>
/// Result of tournament statistics calculation including success status and generated counts.
/// </summary>
public class StatsCalculationResult
{
    /// <summary>
    /// Whether the statistics calculation completed successfully.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Number of PlayerTournamentStats records generated.
    /// </summary>
    public int PlayerTournamentStatsCount { get; init; }

    /// <summary>
    /// Total number of PlayerMatchStats records generated across all matches.
    /// </summary>
    public int PlayerMatchStatsCount { get; init; }

    /// <summary>
    /// Number of verified matches processed.
    /// </summary>
    public int VerifiedMatchesCount { get; init; }

    /// <summary>
    /// Optional error message if calculation failed.
    /// </summary>
    public string? ErrorMessage { get; init; }
}
