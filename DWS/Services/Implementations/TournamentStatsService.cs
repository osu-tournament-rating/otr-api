using Common.Enums.Verification;
using Database.Entities;
using Database.Repositories.Interfaces;
using DWS.Calculators;
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
        bool success = statsCalculator.CalculateAllStatistics(tournament);
        if (!success)
        {
            logger.LogError(
                "Failed to calculate statistics for tournament [Id: {Id}]",
                tournament.Id
            );
            return false;
        }

        // Single database save for all changes
        await tournamentsRepository.UpdateAsync(tournament);

        logger.LogInformation(
            "Successfully processed tournament statistics [Id: {Id} | Matches: {MatchCount} | Player Stats: {PlayerCount}]",
            tournament.Id,
            tournament.Matches.Count(m => m.VerificationStatus == VerificationStatus.Verified),
            tournament.PlayerTournamentStats.Count
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
