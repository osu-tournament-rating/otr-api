using Database.Entities;
using Database.Enums.Verification;

namespace DataWorkerService.AutomationChecks.Games;

/// <summary>
/// Checks for <see cref="Game"/>s with an unexpected count of valid <see cref="GameScore"/>s
/// </summary>
public class GameScoreCountCheck(ILogger<GameScoreCountCheck> logger) : AutomationCheckBase<Game>(logger)
{
    protected override bool OnChecking(Game entity)
    {
        // Game has no scores at all
        if (entity.Scores.Count == 0)
        {
            entity.RejectionReason |= GameRejectionReason.NoScores;
            return false;
        }

        var validScoresCount = entity.Scores
            .Count(gs => gs.VerificationStatus is VerificationStatus.PreVerified or VerificationStatus.Verified);

        // Game has no valid scores
        if (validScoresCount == 0)
        {
            entity.RejectionReason |= GameRejectionReason.NoValidScores;
            return false;
        }

        // Number of scores matches expected team size
        if (validScoresCount % 2 == 0 && validScoresCount / 2 == entity.Match.Tournament.TeamSize)
        {
            return true;
        }

        entity.RejectionReason |= GameRejectionReason.TeamSizeMismatch;
        return false;
    }
}
