using Database.Entities;
using Database.Enums.Verification;

namespace DataWorkerService.AutomationChecks.Matches;

/// <summary>
/// Checks for <see cref="Match"/>es with an unexpected count of valid <see cref="Game"/>s
/// </summary>
public class MatchGameCountCheck(ILogger<MatchGameCountCheck> logger) : AutomationCheckBase<Match>(logger)
{
    public override int Order => 2;

    protected override bool OnChecking(Match entity)
    {
        var validGamesCount = entity.Games
            .Count(g => g.VerificationStatus is VerificationStatus.PreVerified or VerificationStatus.Verified);

        // Match has no valid games
        if (validGamesCount == 0)
        {
            entity.RejectionReason |= MatchRejectionReason.NoVerifiedGames;
            return false;
        }

        // Number of games satisfies a "best of X" situation
        if (validGamesCount % 2 == 0)
        {
            return true;
        }

        entity.RejectionReason |= MatchRejectionReason.UnexpectedGameCount;
        return false;
    }
}
