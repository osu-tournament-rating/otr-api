using Common.Enums.Enums;
using Common.Enums.Enums.Verification;
using Database.Entities;

namespace DataWorkerService.AutomationChecks.Matches;

/// <summary>
/// Checks (and attempts to fix) <see cref="Match"/>es played in a <see cref="Tournament"/> with a
/// <see cref="Tournament.LobbyTeamSize"/> of 1 where all <see cref="Match.Games"/> were played with a
/// <see cref="TeamType"/> of <see cref="TeamType.HeadToHead"/>
/// instead of <see cref="TeamType.TeamVs"/>
/// </summary>
/// <remarks>
/// Functionally this automation check attempts to programatically correct 1v1 Tournament games where the match was
/// mistakenly set to HeadToHead instead of TeamVs by assigning a team to both players
/// </remarks>
public class MatchHeadToHeadCheck(ILogger<MatchHeadToHeadCheck> logger) : AutomationCheckBase<Match>(logger)
{
    public override int Order => 1;

    protected override bool OnChecking(Match entity)
    {
        if (entity.Games.Count == 0)
        {
            logger.LogDebug("Match has no games");
            return true;
        }

        if (entity.Tournament.LobbyTeamSize != 1)
        {
            logger.LogDebug("Match's tournament team size is not 1 [Team size: {TeamSize}]", entity.Tournament.LobbyTeamSize);
            return true;
        }

        if (!entity.Games.All(g =>
                g.VerificationStatus is VerificationStatus.PreRejected
                && g.RejectionReason is GameRejectionReason.InvalidTeamType
                && g.TeamType is TeamType.HeadToHead
                && g.Scores.Count(s => s.VerificationStatus is VerificationStatus.PreVerified) == 2
            )
        )
        {
            logger.LogDebug("Match's games are not eligible for TeamVs conversion");
            return true;
        }

        logger.LogInformation("Attempting to convert HeadToHead games to TeamVs [Id: {Id}]", entity.Id);

        IEnumerable<Game> preRejectedGames = [.. entity.Games.Where(g => g.VerificationStatus is VerificationStatus.PreRejected)];

        // Decide which players are Red and Blue
        var firstGameScores = preRejectedGames.First().Scores
            .Where(s => s.VerificationStatus is VerificationStatus.PreVerified)
            .ToList();

        var bluePlayerOsuId = firstGameScores.ElementAt(0).Player.OsuId;
        var redPlayerOsuId = firstGameScores.ElementAt(1).Player.OsuId;

        if (preRejectedGames.Any(g => g.Scores
                .Where(s => s.VerificationStatus is VerificationStatus.PreVerified)
                .Any(s => s.Player.OsuId != redPlayerOsuId && s.Player.OsuId != bluePlayerOsuId)))
        {
            logger.LogInformation(
                "Match's participants contain more than two unique osu! ids, aborting TeamVs conversion [Id: {Id}]",
                entity.Id
            );

            entity.RejectionReason |= MatchRejectionReason.FailedTeamVsConversion;

            foreach (Game game in preRejectedGames)
            {
                game.RejectionReason |= GameRejectionReason.FailedTeamVsConversion;
            }

            return false;
        }

        // Convert games to TeamVs by assigning teams
        foreach (Game game in preRejectedGames)
        {
            var scores = game.Scores
                .Where(s => s.VerificationStatus is VerificationStatus.PreVerified)
                .ToList();

            GameScore blueScore = scores.First(s => s.Player.OsuId == bluePlayerOsuId);
            GameScore redScore = scores.First(s => s.Player.OsuId == redPlayerOsuId);

            blueScore.Team = Team.Blue;
            redScore.Team = Team.Red;

            game.TeamType = TeamType.TeamVs;
            game.RejectionReason = GameRejectionReason.None;
            game.VerificationStatus = VerificationStatus.PreVerified;
        }

        logger.LogInformation("Successfully converted HeadToHead games to TeamVs [Id: {Id}]", entity.Id);

        return true;
    }
}
