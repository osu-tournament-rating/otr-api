using System.Diagnostics.CodeAnalysis;
using Common.Enums.Enums;
using Common.Enums.Enums.Verification;
using Database.Entities;

namespace TestingUtils.SeededData;

/// <summary>
/// Utility for generating <see cref="Match"/>es with seeded data
/// </summary>
public static class SeededMatch
{
    private static readonly Random s_rand = new();

    /// <summary>
    /// Generates a <see cref="Match"/> with seeded data
    /// </summary>
    /// <remarks>Any properties not given will be randomized</remarks>
    public static Match Generate(
        int? id = null,
        long? osuId = null,
        string? name = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        VerificationStatus? verificationStatus = null,
        MatchRejectionReason? rejectionReason = null,
        MatchProcessingStatus? processingStatus = null,
        MatchWarningFlags? warningFlags = null,
        Tournament? tournament = null
    )
    {
        Tournament seededTournament = tournament ?? SeededTournament.Generate();

        var seededMatch = new Match
        {
            Id = id ?? s_rand.Next(),
            OsuId = osuId ?? s_rand.NextInt64(),
            Name = name ?? string.Empty,
            VerificationStatus = verificationStatus ?? s_rand.NextEnum<VerificationStatus>(),
            RejectionReason = rejectionReason ?? s_rand.NextEnum<MatchRejectionReason>(),
            WarningFlags = warningFlags ?? s_rand.NextEnum<MatchWarningFlags>(),
            ProcessingStatus = processingStatus ?? s_rand.NextEnum<MatchProcessingStatus>(),
            TournamentId = seededTournament.Id,
            Tournament = seededTournament
        };

        if (startTime.HasValue && endTime.HasValue)
        {
            seededMatch.StartTime = startTime.Value;
            seededMatch.EndTime = endTime.Value;
        }

        seededMatch.StartTime = startTime ?? SeededDate.Generate();
        seededMatch.EndTime = endTime ?? SeededDate.GenerateAfter(seededMatch.StartTime);

        seededTournament.Matches.Add(seededMatch);

        return seededMatch;
    }

    /// <summary>
    /// Creates a full real <see cref="Match"/> including all <see cref="Game"/>s, <see cref="GameScore"/>s,
    /// <see cref="Player"/>s, and <see cref="Beatmap"/>s
    /// </summary>
    /// <remarks>The example match is a best of 9 played between United States and Canada in OWC 2023</remarks>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public static Match ExampleMatch()
    {
        Match match = Generate(
            id: 1,
            osuId: 111278789,
            name: "OWC2023: (United States) VS (Canada)",
            startTime: new DateTime(2023, 11, 12, 18, 50, 31),
            endTime: new DateTime(2023, 11, 12, 19, 51, 10)
        );

        #region Players

        Player xooty = SeededPlayer.Generate(
            id: 1,
            osuId: 3717598,
            username: "xootynator",
            country: "CA"
        );

        Player cutpaper = SeededPlayer.Generate(
            id: 2,
            osuId: 10975777,
            username: "CutPaper",
            country: "CA"
        );

        Player zylice = SeededPlayer.Generate(
            id: 3,
            osuId: 5033077,
            username: "Zylice",
            country: "CA"
        );

        Player ryuk = SeededPlayer.Generate(
            id: 4,
            osuId: 6304246,
            username: "Ryuk",
            country: "CA"
        );

        Player tekkito = SeededPlayer.Generate(
            id: 5,
            osuId: 7075211,
            username: "tekkito",
            country: "US"
        );

        Player rektygon = SeededPlayer.Generate(
            id: 6,
            osuId: 7813296,
            username: "hydrogen bomb",
            country: "US"
        );

        Player vaxei = SeededPlayer.Generate(
            id: 7,
            osuId: 4787150,
            username: "Vaxei",
            country: "US"
        );

        Player boshy = SeededPlayer.Generate(
            id: 8,
            osuId: 4830687,
            username: "BoshyMan741",
            country: "US"
        );

        Player fiery = SeededPlayer.Generate(
            id: 9,
            osuId: 3533958,
            username: "fieryrage",
            country: "US"
        );

        Player stoof = SeededPlayer.Generate(
            id: 10,
            osuId: 4916057,
            username: "Stoof",
            country: "CA"
        );

        Player vesp = SeededPlayer.Generate(
            id: 11,
            osuId: 5425046,
            username: "Vespirit",
            country: "CA"
        );

        Player kurtis = SeededPlayer.Generate(
            id: 12,
            osuId: 5477343,
            username: "kurtis-",
            country: "CA"
        );

        Player wudci = SeededPlayer.Generate(
            id: 13,
            osuId: 2590257,
            username: "wudci",
            country: "US"
        );

        Player window = SeededPlayer.Generate(
            id: 14,
            osuId: 4108547,
            username: "WindowLife",
            country: "US"
        );

        Player kama = SeededPlayer.Generate(
            id: 15,
            osuId: 13380270,
            username: "Kama",
            country: "US"
        );

        Player yip = SeededPlayer.Generate(
            id: 16,
            osuId: 5177569,
            username: "Yip",
            country: "CA"
        );

        #endregion

        #region Game 1

        Game game1 = SeededGame.Generate(
            id: 1,
            osuId: 575200385,
            ruleset: Ruleset.Osu,
            scoringType: ScoringType.ScoreV2,
            teamType: TeamType.TeamVs,
            mods: Mods.Hidden | Mods.NoFail,
            startTime: new DateTime(2023, 11, 12, 19, 7, 39),
            endTime: new DateTime(2023, 11, 12, 19, 11, 46),
            match: match,
            beatmap: SeededBeatmap.Generate(
                id: 1,
                osuId: 4368596,
                hasData: true,
                artist: "youman feat. GUMI",
                title: "Weenywalker",
                diffName: "holy flip dude",
                rankedStatus: BeatmapRankedStatus.Graveyard,
                sr: 7.02,
                bpm: 200,
                cs: 4,
                ar: 9.3,
                hp: 5,
                od: 9.3,
                totalLength: 237,
                ruleset: Ruleset.Osu,
                circleCount: 860,
                sliderCount: 391,
                spinnerCount: 2,
                maxCombo: 1694
            )
        );

        SeededScore.Generate(
            id: 1,
            score: 626313,
            maxCombo: 1155,
            count50: 2,
            count100: 26,
            count300: 1220,
            countMiss: 5,
            countKatu: 22,
            countGeki: 174,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.Hidden | Mods.NoFail,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: vaxei,
            game: game1
        );

        SeededScore.Generate(
            id: 2,
            score: 446038,
            maxCombo: 533,
            count50: 8,
            count100: 25,
            count300: 1212,
            countMiss: 8,
            countKatu: 12,
            countGeki: 184,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.Hidden | Mods.NoFail,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: boshy,
            game: game1
        );

        SeededScore.Generate(
            id: 3,
            score: 649341,
            maxCombo: 884,
            count50: 5,
            count100: 20,
            count300: 1227,
            countMiss: 1,
            countKatu: 12,
            countGeki: 186,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.Hidden | Mods.NoFail,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: rektygon,
            game: game1
        );

        SeededScore.Generate(
            id: 4,
            score: 818682,
            maxCombo: 1362,
            count50: 0,
            count100: 4,
            count300: 1247,
            countMiss: 2,
            countKatu: 4,
            countGeki: 196,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.Hidden | Mods.NoFail,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: tekkito,
            game: game1
        );

        SeededScore.Generate(
            id: 5,
            score: 785095,
            maxCombo: 1266,
            count50: 1,
            count100: 4,
            count300: 1248,
            countMiss: 0,
            countKatu: 4,
            countGeki: 196,
            pass: true,
            perfect: false,
            grade: ScoreGrade.SH,
            mods: Mods.Hidden | Mods.NoFail,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: xooty,
            game: game1
        );

        SeededScore.Generate(
            id: 6,
            score: 704034,
            maxCombo: 1210,
            count50: 5,
            count100: 19,
            count300: 1224,
            countMiss: 5,
            countKatu: 14,
            countGeki: 184,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.Hidden | Mods.NoFail,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: zylice,
            game: game1
        );

        SeededScore.Generate(
            id: 7,
            score: 456574,
            maxCombo: 541,
            count50: 2,
            count100: 21,
            count300: 1225,
            countMiss: 5,
            countKatu: 13,
            countGeki: 183,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.Hidden | Mods.NoFail,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: ryuk,
            game: game1
        );

        SeededScore.Generate(
            id: 8,
            score: 763895,
            maxCombo: 1355,
            count50: 0,
            count100: 29,
            count300: 1222,
            countMiss: 2,
            countKatu: 18,
            countGeki: 181,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.Hidden | Mods.NoFail,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: cutpaper,
            game: game1
        );

        #endregion

        #region Game 2

        Game game2 = SeededGame.Generate(
            id: 2,
            osuId: 575200970,
            ruleset: Ruleset.Osu,
            scoringType: ScoringType.ScoreV2,
            teamType: TeamType.TeamVs,
            mods: Mods.NoFail,
            startTime: new DateTime(2023, 11, 12, 19, 13, 54),
            endTime: new DateTime(2023, 11, 12, 19, 15, 45),
            match: match,
            beatmap: SeededBeatmap.Generate(
                id: 2,
                osuId: 3257371,
                hasData: true,
                artist: "tsunamix_underground",
                title: "Period. ~ Seishin no Kousoku to Jiyuu o Tsukamu Jouka (Cut Ver.)",
                diffName: "hidden extra.",
                rankedStatus: BeatmapRankedStatus.Ranked,
                sr: 6.38,
                bpm: 150,
                cs: 3.2,
                ar: 8.5,
                hp: 6,
                od: 9,
                totalLength: 110,
                ruleset: Ruleset.Osu,
                circleCount: 519,
                sliderCount: 110,
                spinnerCount: 0,
                maxCombo: 747
            )
        );

        SeededScore.Generate(
            id: 2197,
            score: 393144,
            maxCombo: 360,
            count50: 7,
            count100: 37,
            count300: 579,
            countMiss: 6,
            countKatu: 32,
            countGeki: 231,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.NoFail,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: vaxei,
            game: game2
        );

        SeededScore.Generate(
            id: 2198,
            score: 454121,
            maxCombo: 267,
            count50: 2,
            count100: 14,
            count300: 609,
            countMiss: 4,
            countKatu: 9,
            countGeki: 258,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.NoFail,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: boshy,
            game: game2
        );

        SeededScore.Generate(
            id: 2199,
            score: 551125,
            maxCombo: 430,
            count50: 2,
            count100: 16,
            count300: 610,
            countMiss: 1,
            countKatu: 12,
            countGeki: 258,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.NoFail,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: fiery,
            game: game2
        );

        SeededScore.Generate(
            id: 2200,
            score: 346012,
            maxCombo: 208,
            count50: 6,
            count100: 23,
            count300: 595,
            countMiss: 5,
            countKatu: 19,
            countGeki: 245,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.NoFail,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: rektygon,
            game: game2
        );

        SeededScore.Generate(
            id: 2201,
            score: 331408,
            maxCombo: 254,
            count50: 3,
            count100: 25,
            count300: 586,
            countMiss: 15,
            countKatu: 19,
            countGeki: 240,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.NoFail,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: xooty,
            game: game2
        );

        SeededScore.Generate(
            id: 2202,
            score: 301599,
            maxCombo: 166,
            count50: 6,
            count100: 25,
            count300: 586,
            countMiss: 12,
            countKatu: 20,
            countGeki: 238,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.NoFail,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: stoof,
            game: game2
        );

        SeededScore.Generate(
            id: 2203,
            score: 314027,
            maxCombo: 252,
            count50: 11,
            count100: 33,
            count300: 571,
            countMiss: 14,
            countKatu: 21,
            countGeki: 234,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.NoFail,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: vesp,
            game: game2
        );

        SeededScore.Generate(
            id: 2204,
            score: 961688,
            maxCombo: 747,
            count50: 0,
            count100: 10,
            count300: 619,
            countMiss: 0,
            countKatu: 8,
            countGeki: 264,
            pass: true,
            perfect: false,
            grade: ScoreGrade.S,
            mods: Mods.NoFail,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: cutpaper,
            game: game2
        );

        #endregion

        #region Game 3

        Game game3 = SeededGame.Generate(
            id: 3,
            osuId: 575201492,
            ruleset: Ruleset.Osu,
            scoringType: ScoringType.ScoreV2,
            teamType: TeamType.TeamVs,
            mods: Mods.Hidden | Mods.NoFail,
            startTime: new DateTime(2023, 11, 12, 19, 19, 36),
            endTime: new DateTime(2023, 11, 12, 19, 22, 43),
            match: match,
            beatmap: SeededBeatmap.Generate(
                id: 3,
                osuId: 4368609,
                hasData: true,
                artist: "yuikonnu",
                title: "Taifuu Ikka? Risan",
                diffName: "I NEED YOU",
                rankedStatus: BeatmapRankedStatus.Graveyard,
                sr: 6.52,
                bpm: 290,
                cs: 4,
                ar: 9.5,
                hp: 5,
                od: 9,
                totalLength: 177,
                ruleset: Ruleset.Osu,
                circleCount: 449,
                sliderCount: 470,
                spinnerCount: 0,
                maxCombo: 1419
            )
        );

        SeededScore.Generate(
            id: 2205,
            score: 705559,
            maxCombo: 1080,
            count50: 2,
            count100: 29,
            count300: 886,
            countMiss: 2,
            countKatu: 19,
            countGeki: 194,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.Hidden | Mods.NoFail,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: vaxei,
            game: game3
        );

        SeededScore.Generate(
            id: 2206,
            score: 1003904,
            maxCombo: 1413,
            count50: 2,
            count100: 18,
            count300: 899,
            countMiss: 0,
            countKatu: 15,
            countGeki: 200,
            pass: true,
            perfect: false,
            grade: ScoreGrade.SH,
            mods: Mods.Hidden | Mods.NoFail,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: fiery,
            game: game3
        );

        SeededScore.Generate(
            id: 2207,
            score: 665106,
            maxCombo: 929,
            count50: 2,
            count100: 7,
            count300: 909,
            countMiss: 1,
            countKatu: 5,
            countGeki: 208,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.Hidden | Mods.NoFail,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: rektygon,
            game: game3
        );

        SeededScore.Generate(
            id: 2208,
            score: 622770,
            maxCombo: 836,
            count50: 2,
            count100: 4,
            count300: 912,
            countMiss: 1,
            countKatu: 4,
            countGeki: 209,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.Hidden | Mods.NoFail,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: tekkito,
            game: game3
        );

        SeededScore.Generate(
            id: 2209,
            score: 1037991,
            maxCombo: 1419,
            count50: 0,
            count100: 8,
            count300: 911,
            countMiss: 0,
            countKatu: 8,
            countGeki: 208,
            pass: true,
            perfect: false,
            grade: ScoreGrade.SH,
            mods: Mods.Hidden | Mods.NoFail,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: xooty,
            game: game3
        );

        SeededScore.Generate(
            id: 2210,
            score: 1026567,
            maxCombo: 1418,
            count50: 0,
            count100: 12,
            count300: 907,
            countMiss: 0,
            countKatu: 10,
            countGeki: 206,
            pass: true,
            perfect: false,
            grade: ScoreGrade.SH,
            mods: Mods.Hidden | Mods.NoFail,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: cutpaper,
            game: game3
        );

        SeededScore.Generate(
            id: 2211,
            score: 456570,
            maxCombo: 582,
            count50: 6,
            count100: 9,
            count300: 901,
            countMiss: 3,
            countKatu: 8,
            countGeki: 200,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.Hidden | Mods.NoFail,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: vesp,
            game: game3
        );

        SeededScore.Generate(
            id: 2212,
            score: 711246,
            maxCombo: 927,
            count50: 1,
            count100: 5,
            count300: 912,
            countMiss: 1,
            countKatu: 5,
            countGeki: 209,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.Hidden | Mods.NoFail,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: ryuk,
            game: game3
        );

        #endregion

        #region Game 4

        Game game4 = SeededGame.Generate(
            id: 4,
            osuId: 575202076,
            ruleset: Ruleset.Osu,
            scoringType: ScoringType.ScoreV2,
            teamType: TeamType.TeamVs,
            mods: Mods.DoubleTime | Mods.NoFail,
            startTime: new DateTime(2023, 11, 12, 19, 26, 3),
            endTime: new DateTime(2023, 11, 12, 19, 28, 43),
            match: match,
            beatmap: SeededBeatmap.Generate(
                id: 4,
                osuId: 4368554,
                hasData: true,
                artist: "Umeri",
                title: "paranoia",
                diffName: "deppy's insane",
                rankedStatus: BeatmapRankedStatus.Graveyard,
                sr: 4.43,
                bpm: 150,
                cs: 3.5,
                ar: 6,
                hp: 7,
                od: 8,
                totalLength: 216,
                ruleset: Ruleset.Osu,
                circleCount: 519,
                sliderCount: 179,
                spinnerCount: 5,
                maxCombo: 939
            )
        );

        SeededScore.Generate(
            id: 2213,
            score: 653139,
            maxCombo: 1083,
            count50: 3,
            count100: 56,
            count300: 1283,
            countMiss: 1,
            countKatu: 35,
            countGeki: 192,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.DoubleTime | Mods.NoFail,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: vaxei,
            game: game4
        );

        SeededScore.Generate(
            id: 2214,
            score: 648051,
            maxCombo: 806,
            count50: 5,
            count100: 15,
            count300: 1321,
            countMiss: 2,
            countKatu: 6,
            countGeki: 222,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.DoubleTime | Mods.NoFail,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: window,
            game: game4
        );

        SeededScore.Generate(
            id: 2215,
            score: 1090315,
            maxCombo: 1638,
            count50: 2,
            count100: 4,
            count300: 1337,
            countMiss: 0,
            countKatu: 4,
            countGeki: 226,
            pass: true,
            perfect: false,
            grade: ScoreGrade.S,
            mods: Mods.DoubleTime | Mods.NoFail,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: rektygon,
            game: game4
        );

        SeededScore.Generate(
            id: 2216,
            score: 677493,
            maxCombo: 1062,
            count50: 4,
            count100: 40,
            count300: 1299,
            countMiss: 0,
            countKatu: 26,
            countGeki: 203,
            pass: true,
            perfect: false,
            grade: ScoreGrade.S,
            mods: Mods.DoubleTime | Mods.NoFail,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: wudci,
            game: game4
        );

        SeededScore.Generate(
            id: 2217,
            score: 1142086,
            maxCombo: 1746,
            count50: 3,
            count100: 26,
            count300: 1314,
            countMiss: 0,
            countKatu: 18,
            countGeki: 212,
            pass: true,
            perfect: false,
            grade: ScoreGrade.S,
            mods: Mods.DoubleTime | Mods.NoFail,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: zylice,
            game: game4
        );

        SeededScore.Generate(
            id: 2218,
            score: 541835,
            maxCombo: 633,
            count50: 5,
            count100: 14,
            count300: 1315,
            countMiss: 9,
            countKatu: 14,
            countGeki: 213,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.DoubleTime | Mods.NoFail,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: vesp,
            game: game4
        );

        SeededScore.Generate(
            id: 2219,
            score: 1025353,
            maxCombo: 1597,
            count50: 0,
            count100: 24,
            count300: 1318,
            countMiss: 1,
            countKatu: 20,
            countGeki: 210,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.DoubleTime | Mods.NoFail,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: ryuk,
            game: game4
        );

        SeededScore.Generate(
            id: 2220,
            score: 505387,
            maxCombo: 708,
            count50: 9,
            count100: 22,
            count300: 1304,
            countMiss: 8,
            countKatu: 15,
            countGeki: 210,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.DoubleTime | Mods.NoFail,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: kurtis,
            game: game4
        );

        #endregion

        #region Game 5

        Game game5 = SeededGame.Generate(
            id: 5,
            osuId: 575202589,
            ruleset: Ruleset.Osu,
            scoringType: ScoringType.ScoreV2,
            teamType: TeamType.TeamVs,
            mods: Mods.None,
            startTime: new DateTime(2023, 11, 12, 19, 32, 0),
            endTime: new DateTime(2023, 11, 12, 19, 35, 45),
            match: match,
            beatmap: SeededBeatmap.Generate(
                id: 5,
                osuId: 4368565,
                hasData: true,
                artist: "Motoki Zakuro",
                title: "Intellectual Rapist ver-C",
                diffName: "The Detective's Soliloquy",
                rankedStatus: BeatmapRankedStatus.Graveyard,
                sr: 7.3,
                bpm: 236,
                cs: 3.6,
                ar: 9.6,
                hp: 6.8,
                od: 9.2,
                totalLength: 269,
                ruleset: Ruleset.Osu,
                circleCount: 744,
                sliderCount: 274,
                spinnerCount: 2,
                maxCombo: 1314
            )
        );

        SeededScore.Generate(
            id: 2221,
            score: 633473,
            maxCombo: 711,
            count50: 0,
            count100: 15,
            count300: 1004,
            countMiss: 1,
            countKatu: 10,
            countGeki: 347,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.NoFail,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: vaxei,
            game: game5
        );

        SeededScore.Generate(
            id: 2222,
            score: 766832,
            maxCombo: 958,
            count50: 1,
            count100: 6,
            count300: 1012,
            countMiss: 1,
            countKatu: 6,
            countGeki: 350,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.Hidden | Mods.NoFail,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: tekkito,
            game: game5
        );

        SeededScore.Generate(
            id: 2223,
            score: 1077259,
            maxCombo: 1314,
            count50: 0,
            count100: 12,
            count300: 1008,
            countMiss: 0,
            countKatu: 11,
            countGeki: 347,
            pass: true,
            perfect: false,
            grade: ScoreGrade.S,
            mods: Mods.NoFail | Mods.HardRock,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: rektygon,
            game: game5
        );

        SeededScore.Generate(
            id: 2224,
            score: 678817,
            maxCombo: 953,
            count50: 1,
            count100: 23,
            count300: 995,
            countMiss: 1,
            countKatu: 17,
            countGeki: 339,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.NoFail,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: wudci,
            game: game5
        );

        SeededScore.Generate(
            id: 2225,
            score: 688633,
            maxCombo: 794,
            count50: 0,
            count100: 10,
            count300: 1009,
            countMiss: 1,
            countKatu: 7,
            countGeki: 350,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.Hidden | Mods.NoFail,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: zylice,
            game: game5
        );

        SeededScore.Generate(
            id: 2226,
            score: 475791,
            maxCombo: 425,
            count50: 0,
            count100: 27,
            count300: 986,
            countMiss: 7,
            countKatu: 17,
            countGeki: 336,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.NoFail | Mods.HardRock,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: ryuk,
            game: game5
        );

        SeededScore.Generate(
            id: 2227,
            score: 1040008,
            maxCombo: 1314,
            count50: 0,
            count100: 13,
            count300: 1007,
            countMiss: 0,
            countKatu: 8,
            countGeki: 350,
            pass: true,
            perfect: false,
            grade: ScoreGrade.SH,
            mods: Mods.Hidden | Mods.NoFail,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: cutpaper,
            game: game5
        );

        SeededScore.Generate(
            id: 2228,
            score: 699710,
            maxCombo: 780,
            count50: 1,
            count100: 8,
            count300: 1010,
            countMiss: 1,
            countKatu: 6,
            countGeki: 350,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.Hidden | Mods.NoFail,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: xooty,
            game: game5
        );

        #endregion

        #region Game 6

        Game game6 = SeededGame.Generate(
            id: 6,
            osuId: 575203265,
            ruleset: Ruleset.Osu,
            scoringType: ScoringType.ScoreV2,
            teamType: TeamType.TeamVs,
            mods: Mods.NoFail | Mods.HardRock,
            startTime: new DateTime(2023, 11, 12, 19, 39, 13),
            endTime: new DateTime(2023, 11, 12, 19, 43, 1),
            match: match,
            beatmap: SeededBeatmap.Generate(
                id: 6,
                osuId: 2040486,
                hasData: true,
                artist: "lapix",
                title: "Horizon Blue feat. Kanata.N",
                diffName: "Endless Expanse",
                rankedStatus: BeatmapRankedStatus.Ranked,
                sr: 6.16,
                bpm: 153,
                cs: 4,
                ar: 9.5,
                hp: 4,
                od: 9,
                totalLength: 220,
                ruleset: Ruleset.Osu,
                circleCount: 558,
                sliderCount: 561,
                spinnerCount: 2,
                maxCombo: 1846
            )
        );

        SeededScore.Generate(
            id: 2229,
            score: 849197,
            maxCombo: 1647,
            count50: 2,
            count100: 45,
            count300: 1074,
            countMiss: 0,
            countKatu: 40,
            countGeki: 272,
            pass: true,
            perfect: false,
            grade: ScoreGrade.S,
            mods: Mods.NoFail | Mods.HardRock,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: vaxei,
            game: game6
        );

        SeededScore.Generate(
            id: 2230,
            score: 552102,
            maxCombo: 945,
            count50: 2,
            count100: 19,
            count300: 1097,
            countMiss: 3,
            countKatu: 17,
            countGeki: 294,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.NoFail | Mods.HardRock,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: kama,
            game: game6
        );

        SeededScore.Generate(
            id: 2231,
            score: 1066646,
            maxCombo: 1844,
            count50: 0,
            count100: 15,
            count300: 1106,
            countMiss: 0,
            countKatu: 13,
            countGeki: 301,
            pass: true,
            perfect: false,
            grade: ScoreGrade.S,
            mods: Mods.NoFail | Mods.HardRock,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: rektygon,
            game: game6
        );

        SeededScore.Generate(
            id: 2232,
            score: 595533,
            maxCombo: 1210,
            count50: 7,
            count100: 41,
            count300: 1067,
            countMiss: 6,
            countKatu: 30,
            countGeki: 277,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.NoFail | Mods.HardRock,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: wudci,
            game: game6
        );

        SeededScore.Generate(
            id: 2233,
            score: 826920,
            maxCombo: 1546,
            count50: 2,
            count100: 22,
            count300: 1096,
            countMiss: 1,
            countKatu: 19,
            countGeki: 293,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.NoFail | Mods.HardRock,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: yip,
            game: game6
        );

        SeededScore.Generate(
            id: 2234,
            score: 719971,
            maxCombo: 1360,
            count50: 0,
            count100: 21,
            count300: 1097,
            countMiss: 3,
            countKatu: 21,
            countGeki: 290,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.NoFail | Mods.HardRock,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: ryuk,
            game: game6
        );

        SeededScore.Generate(
            id: 2235,
            score: 838467,
            maxCombo: 1572,
            count50: 2,
            count100: 21,
            count300: 1098,
            countMiss: 0,
            countKatu: 20,
            countGeki: 292,
            pass: true,
            perfect: false,
            grade: ScoreGrade.S,
            mods: Mods.NoFail | Mods.HardRock,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: zylice,
            game: game6
        );

        SeededScore.Generate(
            id: 2236,
            score: 711109,
            maxCombo: 1205,
            count50: 0,
            count100: 22,
            count300: 1099,
            countMiss: 0,
            countKatu: 21,
            countGeki: 293,
            pass: true,
            perfect: false,
            grade: ScoreGrade.S,
            mods: Mods.NoFail | Mods.HardRock,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: xooty,
            game: game6
        );

        #endregion

        #region Game 7

        Game game7 = SeededGame.Generate(
            id: 7,
            osuId: 575203863,
            ruleset: Ruleset.Osu,
            scoringType: ScoringType.ScoreV2,
            teamType: TeamType.TeamVs,
            mods: Mods.None,
            startTime: new DateTime(2023, 11, 12, 19, 45, 57),
            endTime: new DateTime(2023, 11, 12, 19, 47, 45),
            match: match,
            beatmap: SeededBeatmap.Generate(
                id: 59,
                osuId: 4158054,
                hasData: true,
                artist: "Satoshi Terashima",
                title: "GO GET'EM",
                diffName: "expert",
                rankedStatus: BeatmapRankedStatus.Pending,
                sr: 6.35,
                bpm: 146,
                cs: 4,
                ar: 9.3,
                hp: 5,
                od: 9,
                totalLength: 98,
                ruleset: Ruleset.Osu,
                circleCount: 442,
                sliderCount: 178,
                spinnerCount: 0,
                maxCombo: 802
            )
        );

        SeededScore.Generate(
            id: 2237,
            score: 563852,
            maxCombo: 428,
            count50: 1,
            count100: 23,
            count300: 594,
            countMiss: 2,
            countKatu: 16,
            countGeki: 91,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.NoFail,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: vaxei,
            game: game7
        );

        SeededScore.Generate(
            id: 2238,
            score: 1015076,
            maxCombo: 802,
            count50: 0,
            count100: 11,
            count300: 609,
            countMiss: 0,
            countKatu: 9,
            countGeki: 100,
            pass: true,
            perfect: false,
            grade: ScoreGrade.SH,
            mods: Mods.Hidden | Mods.NoFail,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: tekkito,
            game: game7
        );

        SeededScore.Generate(
            id: 2239,
            score: 746149,
            maxCombo: 638,
            count50: 1,
            count100: 3,
            count300: 614,
            countMiss: 2,
            countKatu: 3,
            countGeki: 104,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.NoFail,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: kama,
            game: game7
        );

        SeededScore.Generate(
            id: 2240,
            score: 632710,
            maxCombo: 573,
            count50: 5,
            count100: 20,
            count300: 591,
            countMiss: 4,
            countKatu: 10,
            countGeki: 93,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.NoFail | Mods.HardRock,
            team: Team.Red,
            ruleset: Ruleset.Osu,
            player: rektygon,
            game: game7
        );

        SeededScore.Generate(
            id: 2241,
            score: 1040181,
            maxCombo: 802,
            count50: 0,
            count100: 5,
            count300: 615,
            countMiss: 0,
            countKatu: 5,
            countGeki: 104,
            pass: true,
            perfect: false,
            grade: ScoreGrade.SH,
            mods: Mods.Hidden | Mods.NoFail,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: cutpaper,
            game: game7
        );

        SeededScore.Generate(
            id: 2242,
            score: 939391,
            maxCombo: 749,
            count50: 1,
            count100: 8,
            count300: 611,
            countMiss: 0,
            countKatu: 5,
            countGeki: 103,
            pass: true,
            perfect: false,
            grade: ScoreGrade.SH,
            mods: Mods.Hidden | Mods.NoFail,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: zylice,
            game: game7
        );

        SeededScore.Generate(
            id: 2243,
            score: 687228,
            maxCombo: 633,
            count50: 7,
            count100: 12,
            count300: 600,
            countMiss: 1,
            countKatu: 10,
            countGeki: 94,
            pass: true,
            perfect: false,
            grade: ScoreGrade.A,
            mods: Mods.NoFail,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: yip,
            game: game7
        );

        SeededScore.Generate(
            id: 2244,
            score: 1090501,
            maxCombo: 801,
            count50: 0,
            count100: 18,
            count300: 602,
            countMiss: 0,
            countKatu: 13,
            countGeki: 96,
            pass: true,
            perfect: false,
            grade: ScoreGrade.SH,
            mods: Mods.NoFail | Mods.Hidden | Mods.HardRock,
            team: Team.Blue,
            ruleset: Ruleset.Osu,
            player: xooty,
            game: game7
        );

        #endregion

        return match;
    }
}
