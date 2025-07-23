using Common.Enums.Verification;
using Database.Entities;
using Database.Repositories.Interfaces;
using DWS.AutomationChecks;

namespace DWS.Services;

/// <summary>
/// Service for processing automation checks on tournaments.
/// </summary>
public class TournamentAutomationCheckService(
    ILogger<TournamentAutomationCheckService> logger,
    ITournamentsRepository tournamentsRepository,
    TournamentAutomationChecks tournamentAutomationChecks) : ITournamentAutomationCheckService
{
    /// <inheritdoc />
    public async Task<bool> ProcessAutomationChecksAsync(int entityId, bool overrideVerifiedState = false)
    {
        logger.LogInformation("Processing automation checks for tournament {TournamentId}", entityId);

        // Fetch the tournament with eager loading of navigation properties
        Tournament? tournament = await tournamentsRepository.GetAsync(entityId, eagerLoad: true);
        if (tournament is null)
        {
            logger.LogWarning("Tournament {TournamentId} not found", entityId);
            return false;
        }

        // Check if the entity needs processing based on current verification status
        if (!overrideVerifiedState &&
            (tournament.VerificationStatus == VerificationStatus.Verified ||
             tournament.VerificationStatus == VerificationStatus.Rejected))
        {
            logger.LogInformation(
                "Skipping automation checks for tournament {TournamentId} with verification status {VerificationStatus}",
                entityId,
                tournament.VerificationStatus);
            return tournament.VerificationStatus == VerificationStatus.Verified;
        }

        // Reset verification status if overriding
        if (overrideVerifiedState)
        {
            tournament.VerificationStatus = VerificationStatus.None;
            logger.LogInformation(
                "Resetting verification status to None for tournament {TournamentId} due to override",
                entityId);
        }

        // Load matches with games and scores for automation checks
        await tournamentsRepository.LoadMatchesWithGamesAndScoresAsync(tournament);

        // Run automation checks
        TournamentRejectionReason rejectionReason = tournamentAutomationChecks.Process(tournament);

        // Update verification status based on the result
        if (rejectionReason == TournamentRejectionReason.None)
        {
            tournament.VerificationStatus = VerificationStatus.PreVerified;
            tournament.RejectionReason = TournamentRejectionReason.None;
            logger.LogInformation("Tournament {TournamentId} passed all automation checks", entityId);
        }
        else
        {
            tournament.VerificationStatus = VerificationStatus.PreRejected;
            tournament.RejectionReason = rejectionReason;
            logger.LogInformation("Tournament {TournamentId} rejected by automation checks: {RejectionReason}", entityId, rejectionReason);
        }

        // Save the updated tournament
        await tournamentsRepository.UpdateAsync(tournament);

        return rejectionReason == TournamentRejectionReason.None;
    }
}
