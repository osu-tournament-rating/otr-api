﻿using Common.Enums;
using Common.Enums.Verification;
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
        Game[] validGames = entity.Games.Where(g => g.VerificationStatus.IsPreVerifiedOrVerified()).ToArray();

        // Generate match rosters without modifying game entities
        ICollection<MatchRoster> matchRosters = RostersHelper.GenerateRosters(validGames);

        // Check for overlapping rosters
        HashSet<int>[] playerIdsPerRoster = matchRosters.Select(mr => mr.Roster.ToHashSet()).ToArray();

        for (int i = 0; i < playerIdsPerRoster.Length; i++)
        {
            for (int j = i + 1; j < playerIdsPerRoster.Length; j++)
            {
                if (!playerIdsPerRoster[i].Overlaps(playerIdsPerRoster[j]))
                {
                    continue;
                }

                entity.WarningFlags |= MatchWarningFlags.OverlappingRosters;
                return true;
            }
        }

        return true;
    }
}
