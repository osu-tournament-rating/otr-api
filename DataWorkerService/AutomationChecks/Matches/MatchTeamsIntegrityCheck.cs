using Common.Enums.Enums;
using Common.Enums.Enums.Verification;
using Common.Utilities.Extensions;
using Database.Entities;

namespace DataWorkerService.AutomationChecks.Matches;

/// <summary>
/// Checks for <see cref="Match"/>es with the same <see cref="Player"/> in both <see cref="Team"/>s
/// </summary>
public class MatchTeamsIntegrityCheck(ILogger<MatchTeamsIntegrityCheck> logger) : AutomationCheckBase<Match>(logger)
{
    public override int Order => 2;

    protected override bool OnChecking(Match entity)
    {
        GameScore[] validScores = [.. entity.Games
            .Where(x => x.VerificationStatus.IsPreVerifiedOrVerified())
            .SelectMany(x => x.Scores)
            .Where(x => x.VerificationStatus.IsPreVerifiedOrVerified())];

        var teamRedPlayers = validScores
            .Where(x => x.Team == Team.Red)
            .Select(x => x.PlayerId)
            .ToHashSet();

        IEnumerable<int> teamBluePlayers = validScores
            .Where(x => x.Team == Team.Blue)
            .Select(x => x.PlayerId);

        if (teamBluePlayers.Any(teamRedPlayers.Contains))
        {
            entity.WarningFlags |= MatchWarningFlags.SamePlayerInBothTeams;
        }

        return true;
    }
}
