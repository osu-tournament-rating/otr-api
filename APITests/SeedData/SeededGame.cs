using API.Entities;
using API.Enums;
using API.Osu.Enums;

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
        var game = new Game
        {
            Id = s_rand.Next(),
            MatchId = matchId,
            BeatmapId = 24245,
            Ruleset = Ruleset.Standard,
            ScoringType = ScoringType.ScoreV2,
            TeamType = TeamType.HeadToHead,
            Mods = Mods.None,
            PostModSr = 6.36389,
            GameId = 502333236,
            VerificationStatus = GameVerificationStatus.PreVerified,
            RejectionReason = null,
            Created = new DateTime(2023, 09, 14),
            StartTime = new DateTime(2023, 03, 10),
            EndTime = new DateTime(2023, 03, 10),
            Updated = new DateTime(2023, 11, 04),
            Beatmap = new Beatmap
            {
                Id = s_rand.Next(),
                Artist = "Bob Jilly Jones",
                BeatmapId = s_rand.Next(),
                Bpm = 170.0,
                MapperId = s_rand.NextInt64(),
                MapperName = "Smithy Lilly",
                Sr = 5.3,
                AimDiff = 3.2,
                SpeedDiff = 5.3 - 3.2,
                Cs = 4.0,
                Ar = 9.3,
                Hp = 5.0,
                Od = 8.0,
                DrainTime = 174,
                Length = 160,
                Title = "SmithyLillyBobbyJonesy",
                DiffName = "Smithy",
                Ruleset = 0,
                CircleCount = 50,
                SliderCount = 5,
                SpinnerCount = 1,
                MaxCombo = 57,
                Created = new DateTime(2020, 1, 1)
            }
        };

        game.MatchScores = SeededMatchScore.GetScoresForGame(game.Id, 4, 4, 0).ToList();
        return game;
    }
}
