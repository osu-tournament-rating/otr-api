using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;

namespace DWS.AutomationChecks;

public class ScoreAutomationChecks(ILogger<ScoreAutomationChecks> logger)
{
    private const int ScoreMinimum = 1000;

    private static readonly IEnumerable<Mods> _invalidMods = [Mods.SuddenDeath, Mods.Perfect, Mods.Relax, Mods.Autoplay, Mods.Relax2];

    public ScoreRejectionReason Process(GameScore score, Ruleset tournamentRuleset)
    {
        logger.LogTrace("Processing score {ScoreId}", score.Id);

        ScoreRejectionReason result = ScoreMinimumCheck(score) | ScoreModCheck(score) | ScoreRulesetCheck(score, tournamentRuleset);

        logger.LogTrace("Score {ScoreId} processed with rejection reason: {RejectionReason}", score.Id, result);
        return result;
    }

    private ScoreRejectionReason ScoreMinimumCheck(GameScore score)
    {
        if (score.Score > ScoreMinimum)
        {
            return ScoreRejectionReason.None;
        }

        logger.LogTrace("Score {ScoreId} failed minimum score check. Score: {PlayerScore}, Minimum: {ScoreMinimum}",
            score.Id, score.Score, ScoreMinimum);
        return ScoreRejectionReason.ScoreBelowMinimum;

    }

    private ScoreRejectionReason ScoreModCheck(GameScore score)
    {
        if (_invalidMods.All(mod => !score.Mods.HasFlag(mod)))
        {
            return ScoreRejectionReason.None;
        }

        logger.LogTrace("Score {ScoreId} failed mod check. Invalid mods present: {Mods}", score.Id, score.Mods);
        return ScoreRejectionReason.InvalidMods;
    }

    private ScoreRejectionReason ScoreRulesetCheck(GameScore score, Ruleset tournamentRuleset)
    {
        if (tournamentRuleset == score.Ruleset)
        {
            return ScoreRejectionReason.None;
        }

        logger.LogTrace("Score {ScoreId} failed ruleset check. Score Ruleset: {ScoreRuleset}, Tournament Ruleset: {TournamentRuleset}",
                        score.Id, score.Ruleset, tournamentRuleset);
        return ScoreRejectionReason.RulesetMismatch;
    }
}
