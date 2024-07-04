using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;

namespace DataWorkerService.AutomationChecks.Matches;

/// <summary>
/// Checks (and attempts to fix) <see cref="Match"/>es where all <see cref="Game"/>s were played with a
/// <see cref="Database.Enums.TeamType"/> of <see cref="Database.Enums.TeamType.HeadToHead"/>
/// instead of <see cref="Database.Enums.TeamType.TeamVs"/>
/// </summary>
public class MatchHeadToHeadCheck(ILogger<MatchHeadToHeadCheck> logger) : AutomationCheckBase<Match>(logger)
{
    public override int Order => 1;

    protected override bool OnChecking(Match entity)
    {
        if (
            !entity.Games.Any(g => g.VerificationStatus is VerificationStatus.PreRejected)
            || !entity.Games
                .Where(g => g.VerificationStatus is VerificationStatus.PreRejected)
                .All(g => g.RejectionReason is GameRejectionReason.InvalidTeamType && g.Scores.Count == 2)
            || entity.Tournament.TeamSize != 1
            )
        {
            return true;
        }

        logger.LogInformation("Attempting to convert HeadToHead games to TeamVs [Id: {Id}]", entity.Id);

        IEnumerable<Game> preRejectedGames = entity.Games
            .Where(g => g.VerificationStatus is VerificationStatus.PreRejected)
            .ToList();

        if (preRejectedGames.Any(g => g.Scores.Count != 2))
        {
            return false;
        }

        // Decide which players are Red and Blue
        Game firstGame = preRejectedGames.First();

        var bluePlayerOsuId = firstGame.Scores.ElementAt(0).Player.OsuId;
        var redPlayerOsuId = firstGame.Scores.ElementAt(1).Player.OsuId;

        if (preRejectedGames.Any(g => g.Scores.Any(s => s.Player.OsuId != redPlayerOsuId || s.Player.OsuId != bluePlayerOsuId)))
        {
            return false;
        }

        // Convert games to TeamVs by assigning teams
        foreach (Game game in preRejectedGames)
        {
            GameScore blueScore = game.Scores.First(s => s.Player.OsuId == bluePlayerOsuId);
            GameScore redScore = game.Scores.First(s => s.Player.OsuId == redPlayerOsuId);

            blueScore.Team = Team.Blue;
            redScore.Team = Team.Red;

            game.TeamType = TeamType.TeamVs;
        }

        return true;
    }
}
