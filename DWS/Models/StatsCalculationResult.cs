namespace DWS.Models;

/// <summary>
/// Result of tournament statistics calculation including success status and generated counts.
/// </summary>
public class StatsCalculationResult
{
    /// <summary>
    /// Whether the statistics calculation completed successfully.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Number of PlayerTournamentStats records generated.
    /// </summary>
    public int PlayerTournamentStatsCount { get; set; }

    /// <summary>
    /// Total number of PlayerMatchStats records generated across all matches.
    /// </summary>
    public int PlayerMatchStatsCount { get; set; }

    /// <summary>
    /// Number of verified matches processed.
    /// </summary>
    public int VerifiedMatchesCount { get; set; }

    /// <summary>
    /// Optional error message if calculation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
