using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;
using Database.Utilities;

namespace TestingUtils.SeededData;

/// <summary>
/// Utility for generating <see cref="GameScore"/>s with seeded data
/// </summary>
public static class SeededScore
{
    private static readonly Random s_rand = new();

    /// <summary>
    /// Generates a <see cref="GameScore"/> with seeded data
    /// </summary>
    /// <remarks>Any properties not given will be randomized</remarks>
    public static GameScore Generate(
        int? id = null,
        long? score = null,
        int? maxCombo = null,
        int? count50 = null,
        int? count100 = null,
        int? count300 = null,
        int? countMiss = null,
        int? countKatu = null,
        int? countGeki = null,
        bool? pass = null,
        bool? perfect = null,
        ScoreGrade? grade = null,
        Mods? mods = null,
        Team? team = null,
        Ruleset? ruleset = null,
        VerificationStatus? verificationStatus = null,
        ScoreRejectionReason? rejectionReason = null,
        ScoreProcessingStatus? processingStatus = null,
        Player? player = null,
        Game? game = null
    )
    {
        Game seededGame = game ?? SeededGame.Generate();
        Player seededPlayer = player ?? SeededPlayer.Generate();

        var seededScore = new GameScore
        {
            Id = id ?? s_rand.Next(),
            Pass = pass ?? s_rand.NextBool(),
            Mods = mods ?? (Mods)s_rand.NextInclusive((int)Mods.Perfect),
            Team = team ?? s_rand.NextEnum<Team>(),
            Ruleset = ruleset ?? s_rand.NextEnum<Ruleset>(),
            VerificationStatus = verificationStatus ?? s_rand.NextEnum<VerificationStatus>(),
            RejectionReason = rejectionReason ?? s_rand.NextEnum<ScoreRejectionReason>(),
            ProcessingStatus = processingStatus ?? s_rand.NextEnum<ScoreProcessingStatus>(),
            GameId = seededGame.Id,
            Game = seededGame,
            PlayerId = seededPlayer.Id,
            Player = seededPlayer
        };

        seededScore.Score = score ?? s_rand.NextInt64(Convert.ToInt64(1_000_000 * ModScoreMultipliers.Get(seededScore.Mods)));

        seededScore.MaxCombo = maxCombo ?? s_rand.NextInclusive(seededScore.Game.Beatmap?.MaxCombo ?? SeededBeatmap.MaxComboMax);
        seededScore.Count300 = count300 ?? s_rand.NextInclusive(seededScore.MaxCombo);
        seededScore.Count100 = count100 ?? s_rand.NextInclusive(seededScore.MaxCombo - seededScore.Count300);
        seededScore.Count50 = count50 ?? s_rand.NextInclusive(seededScore.MaxCombo - seededScore.Count300 - seededScore.Count100);
        seededScore.CountMiss = countMiss ?? seededScore.MaxCombo - seededScore.Count300 - seededScore.Count100 - seededScore.Count50;

        seededScore.CountGeki = countGeki ?? s_rand.NextInclusive(seededScore.Count300);
        seededScore.CountKatu = countKatu ?? s_rand.NextInclusive(seededScore.Count50);

        seededScore.Grade = grade ?? seededScore.Ruleset switch
        {
            Ruleset.Standard => ScoreGradeUtils.DetermineStandardGrade(seededScore.Accuracy / 100, seededScore.Mods, seededScore),
            Ruleset.Taiko => ScoreGradeUtils.DetermineTaikoGrade(seededScore.Accuracy / 100, seededScore.Mods, seededScore),
            Ruleset.Catch => ScoreGradeUtils.DetermineCatchGrade(seededScore.Accuracy / 100, seededScore.Mods),
            _ => ScoreGradeUtils.DetermineManiaGrade(seededScore.Accuracy / 100, seededScore.Mods)
        };

        seededScore.Perfect = perfect ?? seededScore.Grade is ScoreGrade.SS or ScoreGrade.SSH;

        seededPlayer.Scores.Add(seededScore);
        seededGame.Scores.Add(seededScore);

        return seededScore;
    }
}
