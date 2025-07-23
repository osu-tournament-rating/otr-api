using Common.Enums.Verification;
using Database.Entities;
using Database.Repositories.Interfaces;
using DWS.AutomationChecks;

namespace DWS.Services;

/// <summary>
/// Service for processing automation checks on games.
/// </summary>
public class GameAutomationCheckService(
    ILogger<GameAutomationCheckService> logger,
    IGamesRepository gamesRepository,
    GameAutomationChecks gameAutomationChecks) : IGameAutomationCheckService
{
    /// <inheritdoc />
    public async Task<bool> ProcessAutomationChecksAsync(int entityId)
    {
        logger.LogInformation("Processing automation checks for game {GameId}", entityId);

        // Fetch the game with all navigation properties including Scores
        Game? game = await gamesRepository.GetAsync(entityId, verified: false);
        if (game is null)
        {
            logger.LogWarning("Game {GameId} not found", entityId);
            return false;
        }

        // Run automation checks
        GameRejectionReason rejectionReason = gameAutomationChecks.Process(game);

        // Update verification status based on the result
        if (rejectionReason == GameRejectionReason.None)
        {
            game.VerificationStatus = VerificationStatus.PreVerified;
            game.RejectionReason = GameRejectionReason.None;
            logger.LogInformation("Game {GameId} passed all automation checks", entityId);
        }
        else
        {
            game.VerificationStatus = VerificationStatus.PreRejected;
            game.RejectionReason = rejectionReason;
            logger.LogInformation("Game {GameId} rejected by automation checks: {RejectionReason}", entityId, rejectionReason);
        }

        // Save the updated game
        await gamesRepository.UpdateAsync(game);

        return rejectionReason == GameRejectionReason.None;
    }
}
