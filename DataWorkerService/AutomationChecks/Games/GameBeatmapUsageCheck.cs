using Database.Entities;
using Database.Enums.Verification;

namespace DataWorkerService.AutomationChecks.Games;

/// <summary>
/// Checks for unexpected usage counts of a <see cref="Game"/>'s <see cref="Beatmap"/>
/// </summary>
public class GameBeatmapUsageCheck(ILogger<GameBeatmapUsageCheck> logger) : AutomationCheckBase<Game>(logger)
{
    protected override bool OnChecking(Game entity)
    {
        if (entity.Beatmap is null)
        {
            return true;
        }

        Tournament tournament = entity.Match.Tournament;

        // If the tournament has a mappool
        if (tournament.PooledBeatmaps.Count > 0)
        {
            // If the game's map is in the pool
            if (tournament.PooledBeatmaps.Select(b => b.OsuId).Contains(entity.Beatmap.OsuId))
            {
                return true;
            }

            // If the game is not one of the first 2 in the match
            if (entity.Match.Games
                    .OrderByDescending(g => g.StartTime)
                    .Select(g => g.Id)
                    .ToList()
                    .IndexOf(entity.Id) > 1
               )
            {
                entity.WarningFlags |= GameWarningFlags.BeatmapNotInMappool;
            }
            return true;
        }

        if (tournament.Matches
             .SelectMany(m => m.Games)
             .Select(g => g.Beatmap?.OsuId)
             .Count(id => id == entity.Beatmap.OsuId) == 1
            )
        {
            entity.WarningFlags |= GameWarningFlags.BeatmapUsedOnce;
        }

        return true;
    }
}
