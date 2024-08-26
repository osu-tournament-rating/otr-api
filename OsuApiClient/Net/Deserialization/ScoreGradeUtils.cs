using Database.Enums;
using OsuApiClient.Net.Deserialization.ValueConverters;
using OsuApiClient.Net.JsonModels.Osu.Multiplayer;
using DbScoreGradeUtils = Database.Utilities.ScoreGradeUtils;

namespace OsuApiClient.Net.Deserialization;

/// <summary>
/// Helper for determining the <see cref="Database.Enums.ScoreGrade"/> of a <see cref="GameScoreJsonModel"/>
/// </summary>
public static class ScoreGradeUtils
{
    public static ScoreGrade DetermineGrade(GameScoreJsonModel rawScore)
    {
        Mods mods = ModsConverter.Convert(rawScore.Mods);
        return (Ruleset)rawScore.ModeInt switch
        {
            Ruleset.Taiko => DbScoreGradeUtils.DetermineTaikoGrade(rawScore.Accuracy, mods, rawScore.Statistics),
            Ruleset.Catch => DbScoreGradeUtils.DetermineCatchGrade(rawScore.Accuracy, mods),
            Ruleset.Osu => DbScoreGradeUtils.DetermineStandardGrade(rawScore.Accuracy, mods, rawScore.Statistics),
            _ => DbScoreGradeUtils.DetermineManiaGrade(rawScore.Accuracy, mods)
        };
    }
}
