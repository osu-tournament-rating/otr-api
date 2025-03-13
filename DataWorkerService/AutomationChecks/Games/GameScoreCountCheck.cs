using Common.Enums.Enums;
using Common.Enums.Enums.Verification;
using Common.Utilities.Extensions;
using Database.Entities;
using DataWorkerService.Utilities;

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

        GameScore[] validScores = [.. entity.Scores.Where(gs => gs.VerificationStatus.IsPreVerifiedOrVerified())];
        var validScoresCount = validScores.Length;

        // Game has no valid scores
        if (validScoresCount == 0)
        {
            entity.RejectionReason |= GameRejectionReason.NoValidScores;
            return false;
        }

        if (validScoresCount % 2 == 0 && validScoresCount / 2 == entity.Match.Tournament.LobbySize)
        {
            ICollection<GameRoster> rosters = RostersHelper.GenerateRosters(validScores);

            if (rosters.Count > 1 &&
                rosters.DistinctBy(x => x.Roster.Length).Count() == 1)
            {
                return true;
            }
        }

        entity.RejectionReason |= GameRejectionReason.LobbySizeMismatch;
        return false;
    }
}
