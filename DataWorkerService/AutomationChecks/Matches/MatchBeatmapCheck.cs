using Database.Entities;
using Database.Enums.Verification;

namespace DataWorkerService.AutomationChecks.Matches;

/// <summary>
/// If the <see cref="Match"/> has <see cref="Game"/>s beyond the first two in the collection
/// which have a <see cref="GameRejectionReason"/> of BeatmapNotPooled, apply the
/// UnexpectedBeatmapsFound WarningFlag to the <see cref="Match"/>
/// </summary>
public class MatchBeatmapCheck(ILogger<MatchBeatmapCheck> logger) : AutomationCheckBase<Match>(logger)
{
    protected override bool OnChecking(Match entity)
    {
        var games = entity.Games.OrderBy(g => g.StartTime).ToList();
        if (games.Count < 2)
        {
            return true;
        }

        // If any of Games 3 and beyond have a rejection reason of BeatmapNotPooled, return false
        return !games[2..].Any(g => g.RejectionReason.HasFlag(GameRejectionReason.BeatmapNotPooled));
    }

    protected override void OnFail(Match entity)
    {
        entity.WarningFlags |= MatchWarningFlags.UnexpectedBeatmapsFound;
        base.OnFail(entity);
    }
}
