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
        if (tournament.PooledBeatmaps.Count == 0)
        {
            /**
             * Scan all games in the entire tournament.
             * If there is exactly 1 game which uses this beatmap,
             * flag it.
             */
            if (tournament.Matches
                    .SelectMany(m => m.Games)
                    .Select(g => g.Beatmap?.OsuId)
                    .Count(id => id == entity.Beatmap.OsuId) == 1)
            {
                entity.WarningFlags |= GameWarningFlags.BeatmapUsedOnce;
            }

            return true;
        }

        // If the game's map is in the pool, don't mark with any flags
        if (tournament.PooledBeatmaps.Select(b => b.OsuId).Contains(entity.Beatmap.OsuId))
        {
            return true;
        }

        // The tournament has a mappool, but this beatmap is not present in it.
        // Mark as rejected.
        entity.RejectionReason |= GameRejectionReason.BeatmapNotPooled;
        return false;
    }
}
