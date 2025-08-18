using Common.Enums;
using Database;
using Database.Entities;
using DWS.Messages;
using DWS.Services.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace DWS.Services.Implementations;

/// <summary>
/// Service for tracking and managing tournament data fetching completion status
/// </summary>
public class TournamentDataCompletionService(
    ILogger<TournamentDataCompletionService> logger,
    OtrContext context,
    IPublishEndpoint publishEndpoint)
    : ITournamentDataCompletionService
{
    private readonly HashSet<int> _pendingAutomationChecks = [];

    public async Task<bool> CheckAndTriggerAutomationChecksIfCompleteAsync(int tournamentId, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Checking data completion status for tournament {TournamentId}", tournamentId);

        var matches = await context.Matches
            .Where(m => m.TournamentId == tournamentId)
            .Select(m => new { m.Id, m.DataFetchStatus })
            .ToListAsync(cancellationToken);

        if (matches.Count == 0)
        {
            logger.LogDebug("No matches found for tournament {TournamentId}", tournamentId);
            return false;
        }

        bool allMatchesComplete = matches.All(m =>
            m.DataFetchStatus is DataFetchStatus.Fetched or DataFetchStatus.NotFound);

        if (!allMatchesComplete)
        {
            logger.LogDebug("Not all matches have completed fetching for tournament {TournamentId}. Waiting for remaining matches.", tournamentId);
            return false;
        }

        List<int> pooledBeatmapIds = await context.Tournaments
            .Where(t => t.Id == tournamentId)
            .SelectMany(t => t.PooledBeatmaps.Select(b => b.Id))
            .ToListAsync(cancellationToken);

        List<int> gameBeatmapIds = await context.Games
            .Where(g => g.Match.TournamentId == tournamentId && g.BeatmapId != null)
            .Select(g => g.BeatmapId!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);

        HashSet<int> allBeatmapIds = [.. pooledBeatmapIds, .. gameBeatmapIds];

        if (allBeatmapIds.Count > 0)
        {
            var beatmaps = await context.Beatmaps
                .Where(b => allBeatmapIds.Contains(b.Id))
                .Select(b => new { b.Id, b.DataFetchStatus })
                .ToListAsync(cancellationToken);

            bool allBeatmapsComplete = beatmaps.All(b =>
                b.DataFetchStatus is DataFetchStatus.Fetched or DataFetchStatus.NotFound);

            if (!allBeatmapsComplete)
            {
                int completedCount = beatmaps.Count(b =>
                    b.DataFetchStatus is DataFetchStatus.Fetched or DataFetchStatus.NotFound);

                logger.LogDebug("Not all beatmaps have completed fetching for tournament {TournamentId}. " +
                               "Progress: {Progress} (pooled: {PooledCount}, from games: {GameCount}).",
                    tournamentId, $"{completedCount}/{allBeatmapIds.Count}",
                    pooledBeatmapIds.Count, gameBeatmapIds.Count);
                return false;
            }
        }

        lock (_pendingAutomationChecks)
        {
            if (!_pendingAutomationChecks.Add(tournamentId))
            {
                logger.LogDebug("Automation check already pending for tournament {TournamentId}, skipping duplicate.", tournamentId);
                return false;
            }
        }

        try
        {
            // All data is fetched, update tournament dates based on match times
            logger.LogInformation("All data fetched for tournament {TournamentId}. " +
                                  "Updating tournament start & end dates and triggering automation checks.", tournamentId);

            // Update tournament start and end dates based on match start times
            await UpdateTournamentDatesAsync(tournamentId, cancellationToken);

            var automationCheckMessage = new ProcessTournamentAutomationCheckMessage
            {
                TournamentId = tournamentId,
                Priority = MessagePriority.Normal,
                OverrideVerifiedState = false
            };

            await publishEndpoint.Publish(automationCheckMessage, ctx =>
            {
                ctx.SetPriority((byte)automationCheckMessage.Priority);
                ctx.CorrelationId = automationCheckMessage.CorrelationId;
            }, cancellationToken);

            // Keep the tournament in pending state until the automation check is processed
            // The consumer should call back to clear this when complete
            return true;
        }
        catch (Exception ex)
        {
            // On error, remove from pending set
            lock (_pendingAutomationChecks)
            {
                _pendingAutomationChecks.Remove(tournamentId);
            }

            logger.LogError(ex, "Failed to trigger automation checks for tournament {TournamentId}", tournamentId);
            throw;
        }
    }

    public async Task UpdateMatchFetchStatusAsync(int matchId, DataFetchStatus status, CancellationToken cancellationToken = default)
    {
        Match? match = await context.Matches.FindAsync([matchId], cancellationToken);

        if (match is null)
        {
            logger.LogWarning("Match {MatchId} not found when updating fetch status", matchId);
            return;
        }

        match.DataFetchStatus = status;
        await context.SaveChangesAsync(cancellationToken);

        logger.LogDebug("Updated match {MatchId} fetch status to {Status}", matchId, status);

        await CheckAndTriggerAutomationChecksIfCompleteAsync(match.TournamentId, cancellationToken);
    }

    public async Task UpdateBeatmapFetchStatusAsync(int beatmapId, DataFetchStatus status, CancellationToken cancellationToken = default)
    {
        Beatmap? beatmap = await context.Beatmaps.FindAsync([beatmapId], cancellationToken);

        if (beatmap is null)
        {
            logger.LogWarning("Beatmap {BeatmapId} not found when updating fetch status", beatmapId);
            return;
        }

        beatmap.DataFetchStatus = status;
        await context.SaveChangesAsync(cancellationToken);

        logger.LogDebug("Updated beatmap {BeatmapId} fetch status to {Status}", beatmapId, status);

        // Find tournaments where this beatmap is used (either pooled or in games)
        // First, find tournaments where this beatmap is pooled
        List<int> pooledTournamentIds = await context.Tournaments
            .Where(t => t.PooledBeatmaps.Any(b => b.Id == beatmapId))
            .Select(t => t.Id)
            .ToListAsync(cancellationToken);

        // Second, find tournaments where this beatmap is used in games
        List<int> gameTournamentIds = await context.Games
            .Where(g => g.BeatmapId == beatmapId)
            .Select(g => g.Match.TournamentId)
            .Distinct()
            .ToListAsync(cancellationToken);

        // Combine and deduplicate tournament IDs
        HashSet<int> allTournamentIds = [.. pooledTournamentIds, .. gameTournamentIds];

        logger.LogDebug("Beatmap {BeatmapId} is used in {Count} tournament(s)", beatmapId, allTournamentIds.Count);

        foreach (int tournamentId in allTournamentIds)
        {
            await CheckAndTriggerAutomationChecksIfCompleteAsync(tournamentId, cancellationToken);
        }
    }

    /// <summary>
    /// Updates the tournament's start and end dates based on the earliest and latest match start times
    /// </summary>
    /// <param name="tournamentId">The ID of the tournament to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    private async Task UpdateTournamentDatesAsync(int tournamentId, CancellationToken cancellationToken = default)
    {
        // Query the earliest and latest match start times for this tournament
        List<DateTime> matchDates = await context.Matches
            .Where(m => m.TournamentId == tournamentId && m.StartTime.HasValue)
            .Select(m => m.StartTime!.Value)
            .ToListAsync(cancellationToken);

        if (matchDates.Count == 0)
        {
            logger.LogDebug("No matches with start times found for tournament {TournamentId}", tournamentId);
            return;
        }

        DateTime earliestStartTime = matchDates.Min();
        DateTime latestStartTime = matchDates.Max();

        // Update the tournament entity
        Tournament? tournament = await context.Tournaments.FindAsync([tournamentId], cancellationToken);

        if (tournament is null)
        {
            logger.LogWarning("Tournament {TournamentId} not found when updating dates", tournamentId);
            return;
        }

        tournament.StartTime = earliestStartTime;
        tournament.EndTime = latestStartTime;

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Updated tournament {TournamentId} dates: Start={StartTime}, End={EndTime}",
            tournamentId, earliestStartTime, latestStartTime);
    }

    public void ClearPendingAutomationCheck(int tournamentId)
    {
        lock (_pendingAutomationChecks)
        {
            if (_pendingAutomationChecks.Remove(tournamentId))
            {
                logger.LogDebug("Cleared pending automation check flag for tournament {TournamentId}", tournamentId);
            }
        }
    }
}
