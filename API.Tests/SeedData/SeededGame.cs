using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;

namespace APITests.SeedData;

public static class SeededGame
{
    private static readonly Random s_rand = new();

    public static ICollection<Game> Generate(int matchId, int amount)
    {
        var ls = new List<Game>();

        for (var i = 0; i < amount; i++)
        {
            ls.Add(Generate(matchId));
        }

        return ls;
    }

    private static Game Generate(int matchId)
    {
        BeatmapSet set = SeededBeatmapSet.Get();
        var beatmap = new Beatmap
        {
            Id = s_rand.Next(),
            OsuId = s_rand.Next(),
            Bpm = 170.0,
            Sr = 5.3,
            Cs = 4.0,
            Ar = 9.3,
            Hp = 5.0,
            Od = 8.0,
            TotalLength = 160,
            DiffName = "Smithy",
            Ruleset = 0,
            CountCircle = 50,
            CountSlider = 5,
            CountSpinner = 1,
            MaxCombo = 57,
            Created = new DateTime(2020, 1, 1),
            BeatmapSet = set
        };

        var game = new Game
        {
            Id = s_rand.Next(),
            MatchId = matchId,
            BeatmapId = 24245,
            Ruleset = Ruleset.Osu,
            ScoringType = ScoringType.ScoreV2,
            TeamType = TeamType.HeadToHead,
            Mods = Mods.None,
            OsuId = 502333236,
            VerificationStatus = VerificationStatus.PreVerified,
            RejectionReason = GameRejectionReason.None,
            Created = new DateTime(2023, 09, 14),
            StartTime = new DateTime(2023, 03, 10),
            EndTime = new DateTime(2023, 03, 10),
            Updated = new DateTime(2023, 11, 04),
            Beatmap = beatmap
        };

        game.Scores = SeededMatchScore.GetScoresForGame(game.Id, 4, 4, 0).ToList();
        return game;
    }
}
