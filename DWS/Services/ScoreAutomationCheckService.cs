using Common.Enums.Verification;
using Database.Entities;
using Database.Repositories.Interfaces;
using DWS.AutomationChecks;

namespace DWS.Services;

/// <summary>
/// Service for processing automation checks on scores.
/// </summary>
public class ScoreAutomationCheckService(
    ILogger<ScoreAutomationCheckService> logger,
    IGameScoresRepository gameScoresRepository,
    ScoreAutomationChecks scoreAutomationChecks) : IScoreAutomationCheckService
{
    /// <inheritdoc />
    public async Task<bool> ProcessAutomationChecksAsync(int entityId)
    {
        logger.LogInformation("Processing automation checks for score {ScoreId}", entityId);

        // Fetch the score with its parent entities (Game -> Match -> Tournament)
        GameScore? score = await gameScoresRepository.GetAsync(entityId);
        if (score is null)
        {
            logger.LogWarning("Score {ScoreId} not found", entityId);
            return false;
        }

        // Run automation checks
        ScoreRejectionReason rejectionReason = scoreAutomationChecks.Process(score);

        // Update verification status based on the result
        if (rejectionReason == ScoreRejectionReason.None)
        {
            score.VerificationStatus = VerificationStatus.PreVerified;
            score.RejectionReason = ScoreRejectionReason.None;
            logger.LogInformation("Score {ScoreId} passed all automation checks", entityId);
        }
        else
        {
            score.VerificationStatus = VerificationStatus.PreRejected;
            score.RejectionReason = rejectionReason;
            logger.LogInformation("Score {ScoreId} rejected by automation checks: {RejectionReason}", entityId, rejectionReason);
        }

        // Save the updated score
        await gameScoresRepository.UpdateAsync(score);

        return rejectionReason == ScoreRejectionReason.None;
    }
}
