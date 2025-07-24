namespace DWS.Services;

/// <summary>
/// Service for processing automation checks on tournaments.
/// Orchestrates all child entity checks (scores, games, matches) before running tournament-level checks.
/// </summary>
public interface ITournamentAutomationCheckService
{
    /// <summary>
    /// Processes automation checks for the specified tournament and updates its verification status.
    /// Also processes all child entities (matches, games, scores) within the tournament.
    /// </summary>
    /// <param name="entityId">The ID of the tournament to process</param>
    /// <param name="overrideVerifiedState">Whether to override existing human-verified or rejected states</param>
    /// <returns>True if the tournament passed all automation checks, false otherwise</returns>
    Task<bool> ProcessAutomationChecksAsync(int entityId, bool overrideVerifiedState = false);
}
