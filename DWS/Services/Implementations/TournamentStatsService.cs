using Common.Enums.Verification;
using Database.Entities;
using Database.Repositories.Interfaces;
using DWS.Calculators;
using DWS.Models;
using DWS.Services.Interfaces;

namespace DWS.Services.Implementations;

/// <summary>
/// Service for processing tournament statistics.
/// </summary>
public class TournamentStatsService(
    ILogger<TournamentStatsService> logger,
    ITournamentsRepository tournamentsRepository,
    IStatsCalculator statsCalculator) : ITournamentStatsService
{
    /// <inheritdoc />
    public async Task<bool> ProcessTournamentStatsAsync(int tournamentId)
    {
        // Load entire tournament with all related data in a single query
        Tournament? tournament = await LoadTournamentWithAllDataAsync(tournamentId);
        if (tournament == null)
        {
            return false;
        }

        // Validate tournament is verified
        if (tournament.VerificationStatus is not VerificationStatus.Verified)
        {
            logger.LogError(
                "Stats processing triggered for unverified tournament [Id: {Id} | Status: {Status}]",
                tournament.Id,
                tournament.VerificationStatus
            );
            return false;
        }

        // Perform all statistics calculations in-memory
        StatsCalculationResult result = statsCalculator.CalculateAllStatistics(tournament);
        if (!result.Success)
        {
            logger.LogError(
                "Failed to calculate statistics for tournament {TournamentId}: {Error}",
                tournament.Id,
                result.ErrorMessage ?? "Unknown error"
            );
            return false;
        }

        // Single database save for all changes
        await tournamentsRepository.UpdateAsync(tournament);

        logger.LogInformation(
            "Tournament {TournamentId} statistics processed successfully [Matches: {MatchCount} | PlayerTournamentStats: {PlayerTournamentStatsCount} | PlayerMatchStats: {PlayerMatchStatsCount}]",
            tournament.Id,
            result.VerifiedMatchesCount,
            result.PlayerTournamentStatsCount,
            result.PlayerMatchStatsCount
        );

        return true;
    }

    /// <summary>
    /// Loads a tournament with all related data in a single efficient query.
    /// </summary>
    /// <param name="tournamentId">The tournament ID to load.</param>
    /// <returns>The fully loaded tournament or null if not found.</returns>
    private async Task<Tournament?> LoadTournamentWithAllDataAsync(int tournamentId)
    {
        // Fetch the tournament with basic eager loading first
        Tournament? tournament = await tournamentsRepository.GetAsync(tournamentId, true);

        if (tournament == null)
        {
            logger.LogError("Tournament not found [Id: {Id}]", tournamentId);
            return null;
        }

        // Load all related data in a single comprehensive query operation
        await tournamentsRepository.LoadMatchesWithGamesAndScoresAsync(tournament);

        return tournament;
    }

}
