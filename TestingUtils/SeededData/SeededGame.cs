using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;

namespace TestingUtils.SeededData;

/// <summary>
/// Utility for generating <see cref="Game"/>s with seeded data
/// </summary>
public static class SeededGame
{
    private static readonly Random s_rand = new();

    /// <summary>
    /// Generates a <see cref="Game"/> with seeded data
    /// </summary>
    /// <remarks>Any properties not given will be randomized</remarks>
    public static Game Generate(
        int? id = null,
        long? osuId = null,
        Ruleset? ruleset = null,
        ScoringType? scoringType = null,
        TeamType? teamType = null,
        Mods? mods = null,
        double? postModSr = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        VerificationStatus? verificationStatus = null,
        GameRejectionReason? rejectionReason = null,
        GameProcessingStatus? processingStatus = null,
        Match? match = null,
        Beatmap? beatmap = null
    )
    {
        Match seededMatch = match ?? SeededMatch.Generate();
        Beatmap seededBeatmap = beatmap ?? SeededBeatmap.Generate();

        var seededGame = new Game
        {
            Id = id ?? s_rand.Next(),
            OsuId = osuId ?? s_rand.NextInt64(),
            Ruleset = ruleset ?? s_rand.NextEnum<Ruleset>(),
            ScoringType = scoringType ?? s_rand.NextEnum<ScoringType>(),
            TeamType = teamType ?? s_rand.NextEnum<TeamType>(),
            Mods = mods ?? (Mods)s_rand.NextInclusive((int)Mods.Perfect),
            PostModSr = postModSr ?? s_rand.NextDouble(SeededBeatmap.SrMin, SeededBeatmap.SrMax),
            VerificationStatus = verificationStatus ?? s_rand.NextEnum<VerificationStatus>(),
            RejectionReason = rejectionReason ?? s_rand.NextEnum<GameRejectionReason>(),
            ProcessingStatus = processingStatus ?? s_rand.NextEnum<GameProcessingStatus>(),
            MatchId = seededMatch.Id,
            Match = seededMatch,
            BeatmapId = seededBeatmap.Id,
            Beatmap = seededBeatmap
        };

        if (startTime.HasValue && endTime.HasValue)
        {
            seededGame.StartTime = startTime.Value;
            seededGame.EndTime = endTime.Value;
        }

        seededGame.StartTime = startTime ?? SeededDate.Generate();
        seededGame.EndTime = endTime ?? SeededDate.GenerateAfter(seededGame.StartTime);

        seededMatch.Games.Add(seededGame);
        seededBeatmap.Games.Add(seededGame);

        return seededGame;
    }
}
