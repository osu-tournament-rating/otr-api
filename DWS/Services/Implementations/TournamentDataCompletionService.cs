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
    /// <inheritdoc />
    public async Task<bool> CheckAndTriggerAutomationChecksIfCompleteAsync(int tournamentId, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Checking data completion status for tournament {TournamentId}", tournamentId);

        // Check if all matches for this tournament have their data fetched
        var matches = await context.Matches
            .Where(m => m.TournamentId == tournamentId)
            .Select(m => new { m.Id, m.DataFetchStatus })
            .ToListAsync(cancellationToken);

        if (matches.Count == 0)
        {
            logger.LogDebug("No matches found for tournament {TournamentId}", tournamentId);
            return false;
        }

        // Check if all matches have completed fetching (either Fetched or NotFound are acceptable)
        bool allMatchesComplete = matches.All(m =>
            m.DataFetchStatus is DataFetchStatus.Fetched or DataFetchStatus.NotFound);

        if (!allMatchesComplete)
        {
            logger.LogDebug("Not all matches have completed fetching for tournament {TournamentId}. Waiting for remaining matches.", tournamentId);
            return false;
        }

        // Get all pooled beatmap IDs for this tournament
        List<int> pooledBeatmapIds = await context.Tournaments
            .Where(t => t.Id == tournamentId)
            .SelectMany(t => t.PooledBeatmaps.Select(b => b.Id))
            .ToListAsync(cancellationToken);

        if (pooledBeatmapIds.Count > 0)
        {
            // Check if all pooled beatmaps have their data fetched
            var beatmaps = await context.Beatmaps
                .Where(b => pooledBeatmapIds.Contains(b.Id))
                .Select(b => new { b.Id, b.DataFetchStatus })
                .ToListAsync(cancellationToken);

            bool allBeatmapsComplete = beatmaps.All(b =>
                b.DataFetchStatus is DataFetchStatus.Fetched or DataFetchStatus.NotFound);

            if (!allBeatmapsComplete)
            {
                int completedCount = beatmaps.Count(b =>
                    b.DataFetchStatus is DataFetchStatus.Fetched or DataFetchStatus.NotFound);

                logger.LogDebug("Not all pooled beatmaps have completed fetching for tournament {TournamentId}. Progress: {Progress}.",
                    tournamentId, $"{completedCount}/{pooledBeatmapIds.Count}");
                return false;
            }
        }

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

        return true;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

        // Find tournaments where this beatmap is pooled
        var pooledTournamentIds = await context.Tournaments
            .Where(t => t.PooledBeatmaps.Any(b => b.Id == beatmapId))
            .Select(t => t.Id)
            .ToListAsync(cancellationToken);

        foreach (int tournamentId in pooledTournamentIds)
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
        var matchDates = await context.Matches
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
}
