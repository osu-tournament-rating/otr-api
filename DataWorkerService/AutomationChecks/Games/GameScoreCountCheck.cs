using Common.Enums.Enums;
using Common.Enums.Enums.Verification;
using Database.Entities;

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

        if (entity.TeamType is TeamType.TeamVs or TeamType.TagTeamVs)
        {
            var redValidScoresCount = entity.Scores
                .Count(gs => gs is { Team: Team.Red, VerificationStatus: VerificationStatus.PreVerified or VerificationStatus.Verified });
            var blueValidScoresCount = entity.Scores
                .Count(gs => gs is { Team: Team.Blue, VerificationStatus: VerificationStatus.PreVerified or VerificationStatus.Verified });

            if (redValidScoresCount == blueValidScoresCount &&
                redValidScoresCount == entity.Match.Tournament.LobbyTeamSize &&
                redValidScoresCount + blueValidScoresCount == validScoresCount)
            {
                return true;
            }
        }
        else if (validScoresCount % 2 == 0 && validScoresCount / 2 == entity.Match.Tournament.LobbyTeamSize)
        {
            return true;
        }

        entity.RejectionReason |= GameRejectionReason.LobbySizeMismatch;
        return false;
    }
}
