using Common.Enums.Enums;
using Common.Enums.Enums.Verification;
using Common.Utilities.Extensions;
using Database.Entities;
using DataWorkerService.Utilities;

namespace DataWorkerService.AutomationChecks.Matches;

/// <summary>
/// Checks for <see cref="Match"/>es with the same <see cref="Player"/> among <see cref="Team"/>s
/// </summary>
public class MatchTeamsIntegrityCheck(ILogger<MatchTeamsIntegrityCheck> logger) : AutomationCheckBase<Match>(logger)
{
    public override int Order => 2;

    protected override bool OnChecking(Match entity)
    {
        Game[] validGames = [.. entity.Games.Where(x => x.VerificationStatus.IsPreVerifiedOrVerified())];
        foreach (Game game in validGames)
        {
            IEnumerable<GameScore> validScores = game.Scores.Where(x => x.VerificationStatus.IsPreVerifiedOrVerified());
            game.Rosters = RostersHelper.GenerateRosters(validScores);
        }

        ICollection<MatchRoster> rosters = RostersHelper.GenerateRosters(validGames);
        HashSet<int>[] playerIdsPerRoster = [.. rosters.Select(x => x.Roster.ToHashSet())];

        if (playerIdsPerRoster.Length == 1)
        {
            return true;
        }

        for (var i = 0; i < playerIdsPerRoster.Length; ++i)
        {
            for (var j = i + 1; j < playerIdsPerRoster.Length; ++j)
            {
                if (playerIdsPerRoster[i].Overlaps(playerIdsPerRoster[j]))
                {
                    entity.WarningFlags |= MatchWarningFlags.RostersNotUnique;
                    return true;
                }
            }
        }

        return true;
    }
}
