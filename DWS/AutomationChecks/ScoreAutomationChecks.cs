using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;

namespace DWS.AutomationChecks;

public class ScoreAutomationChecks(ILogger<ScoreAutomationChecks> logger)
{
    private const int ScoreMinimum = 1000;

    /// <summary>
    /// Mods that are ineligible for ratings
    /// </summary>
    private static readonly IEnumerable<Mods> _invalidMods = [Mods.SuddenDeath, Mods.Perfect, Mods.Relax, Mods.Autoplay, Mods.Relax2];

    public static ScoreRejectionReason Process(GameScore score)
    {
        return ScoreMinimumCheck(score) | ScoreModCheck(score) | ScoreRulesetCheck(score);
    }

    private static ScoreRejectionReason ScoreMinimumCheck(GameScore score)
    {
        return score.Score <= ScoreMinimum ? ScoreRejectionReason.ScoreBelowMinimum : ScoreRejectionReason.None;
    }

    /// <summary>
    /// Checks whether the score features invalid mods.
    /// </summary>
    private static ScoreRejectionReason ScoreModCheck(GameScore score)
    {
        return _invalidMods.All(mod => !score.Mods.HasFlag(mod)) ? ScoreRejectionReason.None : ScoreRejectionReason.InvalidMods;
    }

    private static ScoreRejectionReason ScoreRulesetCheck(GameScore score)
    {
        return score.Game.Match.Tournament.Ruleset == score.Ruleset ? ScoreRejectionReason.None : ScoreRejectionReason.RulesetMismatch;
    }
}
