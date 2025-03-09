using Common.Enums.Enums.Verification;
using Database.Entities;

namespace DataWorkerService.AutomationChecks.Tournaments;

/// <summary>
/// Checks for <see cref="Tournament"/>s with an unexpected count of valid <see cref="Match"/>es
/// </summary>
public class TournamentMatchCountCheck(
    ILogger<TournamentMatchCountCheck> logger
) : AutomationCheckBase<Tournament>(logger)
{
    protected override bool OnChecking(Tournament entity)
    {
        var validMatchesCount = entity.Matches.Count(m =>
            m.VerificationStatus is VerificationStatus.PreVerified or VerificationStatus.Verified);

        // Tournament has no valid matches
        if (validMatchesCount == 0)
        {
            entity.RejectionReason |= TournamentRejectionReason.NoVerifiedMatches;
            return false;
        }

        var matchesWithGamesCount = entity.Matches.Count(m => m.RejectionReason != MatchRejectionReason.NoGames);

        // Number of valid matches is above the threshold
        if (validMatchesCount / (double)matchesWithGamesCount >= Constants.TournamentVerifiedMatchesPercentageThreshold)
        {
            return true;
        }

        entity.RejectionReason |= TournamentRejectionReason.NotEnoughVerifiedMatches;
        return false;
    }
}
