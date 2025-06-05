using Common.Enums.Verification;
using Common.Utilities.Extensions;
using Database.Entities;

namespace DataWorkerService.AutomationChecks.Matches;

/// <summary>
/// Checks for <see cref="Match"/>es with an unexpected count of valid <see cref="Game"/>s
/// </summary>
public class MatchGameCountCheck(ILogger<MatchGameCountCheck> logger) : AutomationCheckBase<Match>(logger)
{
    public override int Order => 2;

    protected override bool OnChecking(Match entity)
    {
        // Match has no games at all
        if (entity.Games.Count == 0)
        {
            entity.RejectionReason |= MatchRejectionReason.NoGames;
            return false;
        }

        int validGamesCount = entity.Games
            .Count(g => g.VerificationStatus.IsPreVerifiedOrVerified());

        switch (validGamesCount)
        {
            // Match has no valid games
            case 0:
                entity.RejectionReason |= MatchRejectionReason.NoValidGames;
                return false;
            case < 3:
                entity.RejectionReason |= MatchRejectionReason.UnexpectedGameCount;
                return false;
            case 3 or 4:
                entity.WarningFlags |= MatchWarningFlags.LowGameCount;
                return true;
            // Number of games satisfies a "best of X" situation
            // This turned out to be not that worth to calculate, so as long as there are >= 3 games,
            // it is at least good enough to be sent to manual review
            default:
                return true;
        }
    }
}
