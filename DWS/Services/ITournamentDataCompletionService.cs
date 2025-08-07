using Common.Enums;

namespace DWS.Services;

/// <summary>
/// Service for tracking and managing tournament data fetching completion status
/// </summary>
public interface ITournamentDataCompletionService
{
    /// <summary>
    /// Checks if all data fetching for a tournament is complete and triggers automation checks if so
    /// </summary>
    /// <param name="tournamentId">The tournament ID to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if automation checks were triggered, false otherwise</returns>
    Task<bool> CheckAndTriggerAutomationChecksIfCompleteAsync(int tournamentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a match's data fetch status
    /// </summary>
    /// <param name="matchId">The match ID</param>
    /// <param name="status">The fetch status to set</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateMatchFetchStatusAsync(int matchId, DataFetchStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a beatmap's data fetch status
    /// </summary>
    /// <param name="beatmapId">The beatmap ID</param>
    /// <param name="status">The fetch status to set</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateBeatmapFetchStatusAsync(int beatmapId, DataFetchStatus status, CancellationToken cancellationToken = default);
}
