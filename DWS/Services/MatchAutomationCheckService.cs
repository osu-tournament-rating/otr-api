using Common.Enums.Verification;
using Database.Entities;
using Database.Repositories.Interfaces;
using DWS.AutomationChecks;

namespace DWS.Services;

/// <summary>
/// Service for processing automation checks on matches.
/// </summary>
public class MatchAutomationCheckService(
    ILogger<MatchAutomationCheckService> logger,
    IMatchesRepository matchesRepository,
    MatchAutomationChecks matchAutomationChecks) : IMatchAutomationCheckService
{
    /// <inheritdoc />
    public async Task<bool> ProcessAutomationChecksAsync(int entityId, bool overrideVerifiedState = false)
    {
        logger.LogInformation("Processing automation checks for match {MatchId}", entityId);

        // Fetch the match with all navigation properties including Games with Scores
        Match? match = await matchesRepository.GetFullAsync(entityId);
        if (match is null)
        {
            logger.LogWarning("Match {MatchId} not found", entityId);
            return false;
        }

        // Check if the entity needs processing based on verification status
        if (!overrideVerifiedState && (match.VerificationStatus == VerificationStatus.Verified || match.VerificationStatus == VerificationStatus.Rejected))
        {
            logger.LogInformation("Match {MatchId} already has verification status {Status}, skipping automation checks", entityId, match.VerificationStatus);
            return match.VerificationStatus == VerificationStatus.Verified;
        }

        // If overriding verified state, reset the verification status
        if (overrideVerifiedState)
        {
            match.VerificationStatus = VerificationStatus.None;
            logger.LogInformation("Overriding verification status for match {MatchId}, resetting to None", entityId);
        }

        // Run automation checks
        MatchRejectionReason rejectionReason = matchAutomationChecks.Process(match);

        // Update verification status based on the result
        if (rejectionReason == MatchRejectionReason.None)
        {
            match.VerificationStatus = VerificationStatus.PreVerified;
            match.RejectionReason = MatchRejectionReason.None;
            logger.LogInformation("Match {MatchId} passed all automation checks", entityId);
        }
        else
        {
            match.VerificationStatus = VerificationStatus.PreRejected;
            match.RejectionReason = rejectionReason;
            logger.LogInformation("Match {MatchId} rejected by automation checks: {RejectionReason}", entityId, rejectionReason);
        }

        // Save the updated match
        await matchesRepository.UpdateAsync(match);

        return rejectionReason == MatchRejectionReason.None;
    }
}
