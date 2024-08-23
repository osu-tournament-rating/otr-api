// using System.Diagnostics.CodeAnalysis;
// using API.Osu.AutomationChecks;
// using Database.Entities;
// using Database.Enums;
// using Database.Enums.Verification;
// using Database.Repositories.Interfaces;
// using Moq;
// using Match = Database.Entities.Match;
//
// namespace API.Tests.AutomationChecks;
//
// [SuppressMessage("Usage", "xUnit1031:Do not use blocking task operations in test method")]
// public class AutomationChecksTests
// {
//     private readonly Mock<IMatchesRepository> _matchesServiceMock = new();
//
//     public AutomationChecksTests()
//     {
//         var tournament = new Tournament
//         {
//             Id = 1,
//             Name = "Stage's Tranquility Tournament 3",
//             Abbreviation = "STT3",
//             TeamSize = 1,
//             Ruleset = 0,
//             ForumUrl = "https://osu.ppy.sh/community/forums/topics/1567938?n=1",
//             RankRangeLowerBound = 10000
//         };
//
//         var match = new Match
//         {
//             OsuId = 1,
//             Created = DateTime.UtcNow,
//             StartTime = new DateTime(2023, 1, 1, 0, 0, 0),
//             EndTime = new DateTime(2023, 1, 1, 1, 0, 0),
//             Name = "STT3: (the voices are back) vs (la planta)",
//             VerificationStatus = VerificationStatus.None,
//             Tournament = tournament,
//             TournamentId = tournament.Id
//         };
//
//         var games = new List<Game>
//         {
//             new()
//             {
//                 BeatmapId = 1,
//                 Ruleset = Ruleset.Standard,
//                 StartTime = new DateTime(2023, 1, 1, 0, 0, 0),
//                 EndTime = new DateTime(2023, 1, 1, 0, 1, 0),
//                 GameId = 1,
//                 MatchId = 1,
//                 ScoringType = ScoringType.ScoreV2,
//                 TeamType = TeamType.TeamVs,
//                 Mods = Mods.NoFail | Mods.DoubleTime,
//                 Match = match
//             }
//         };
//
//         var scores = new List<GameScore>
//         {
//             new()
//             {
//                 Id = 1,
//                 Count300 = 890,
//                 Count100 = 75,
//                 Count50 = 4,
//                 CountMiss = 3,
//                 MaxCombo = 620,
//                 Score = 489984,
//                 Team = (int)Team.Red,
//                 Game = games.First(),
//                 IsValid = true
//             },
//             new()
//             {
//                 Id = 2,
//                 Count300 = 902,
//                 Count100 = 53,
//                 Count50 = 6,
//                 CountMiss = 11,
//                 MaxCombo = 545,
//                 Score = 400720,
//                 Team = (int)Team.Blue,
//                 Game = games.First(),
//                 IsValid = true
//             }
//         };
//
//         games.First().MatchScores = scores;
//         match.Games = games;
//
//         _matchesServiceMock
//             .Setup(x => x.GetMatchesNeedingAutoCheckAsync(10000).Result)
//             .Returns(new List<Match> { match });
//     }
//
//     [Fact]
//     public async Task Mock_ReturnsCorrectObject()
//     {
//         Match match = (await _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync()).First();
//
//         Assert.NotNull(match);
//         Assert.Equal("STT3: (the voices are back) vs (la planta)", match.Name);
//
//         Game firstGame = match.Games.First();
//         Assert.NotNull(firstGame);
//         Assert.Equal(Mods.NoFail | Mods.DoubleTime, firstGame.Mods);
//
//         GameScore firstMatchScore = firstGame.MatchScores.First();
//         Assert.NotNull(firstMatchScore);
//         Assert.Equal(890, firstMatchScore.Count300);
//         Assert.Equal(2, firstGame.MatchScores.Count);
//     }
//
//     // Matches
//
//     /// <summary>
//     ///  Tests the flow of a match being processed for
//     /// </summary>
//     [Fact]
//     public void Match_PassesAutomatedChecks()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         Assert.True(MatchAutomationChecks.PassesAllChecks(match));
//     }
//
//     [Fact]
//     public void Match_GameModeValid()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         Assert.True(MatchAutomationChecks.ValidGameMode(match));
//     }
//
//     // TODO: Remove, impossible test case
//     // [Fact]
//     // public void Match_FailsTournamentCheck_WhenMismatchedIds()
//     // {
//     //     Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//     //     match.TournamentId = 5;
//     //     match.Tournament.Id = 6;
//     //     Assert.False(MatchAutomationChecks.HasTournament(match));
//     // }
//     //
//     // [Fact]
//     // public void Match_PassesTournamentCheck_WhenIdsMatch()
//     // {
//     //     Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//     //     match.TournamentId = 5;
//     //     match.Tournament.Id = 5;
//     //     Assert.True(MatchAutomationChecks.HasTournament(match));
//     // }
//
//     [Fact]
//     public void Match_FailsNameCheck()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Name = "STT4: (the voices are back) vs (la planta)";
//         Assert.False(MatchAutomationChecks.PassesNameCheck(match));
//     }
//
//     [Fact]
//     public void Match_FailsNameCheck_WithNullAbbreviation()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Tournament.Abbreviation = string.Empty;
//         Assert.False(MatchAutomationChecks.PassesNameCheck(match));
//     }
//
//     [Fact]
//     public void Match_FailsNameCheck_WithEmptyAbbreviation()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Tournament.Abbreviation = string.Empty;
//         Assert.False(MatchAutomationChecks.PassesNameCheck(match));
//     }
//
//     [Fact]
//     public void Match_FailsNameCheck_WithNullName()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Name = null;
//         Assert.False(MatchAutomationChecks.PassesNameCheck(match));
//     }
//
//     [Fact]
//     public void Match_FailsNameCheck_WithEmptyName()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Name = string.Empty;
//         Assert.False(MatchAutomationChecks.PassesNameCheck(match));
//     }
//
//     [Fact]
//     public void Match_NameCheck_ReturnsFalse_WhenNullAbbreviation()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Tournament.Abbreviation = string.Empty;
//         Assert.False(MatchAutomationChecks.PassesNameCheck(match));
//     }
//
//     [Fact]
//     public void Match_NameCheck_ReturnsFalse_WhenEmptyAbbreviation()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Tournament.Abbreviation = string.Empty;
//         Assert.False(MatchAutomationChecks.PassesNameCheck(match));
//     }
//
//     [Fact]
//     public void Match_NameCheck_ReturnsFalse_WhenNullName()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Name = null;
//         Assert.False(MatchAutomationChecks.PassesNameCheck(match));
//     }
//
//     [Fact]
//     public void Match_NameCheck_ReturnsFalse_WhenEmptyName()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Name = string.Empty;
//         Assert.False(MatchAutomationChecks.PassesNameCheck(match));
//     }
//
//     [Fact]
//     public void Match_NameCheck_ReturnsFalse_WhenNameDoesNotStartWithAbbreviation()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Name = "STT4: (the voices are back) vs (la planta)";
//         match.Tournament.Abbreviation = "STT3";
//         Assert.False(MatchAutomationChecks.PassesNameCheck(match));
//     }
//
//     [Fact]
//     public void Match_NameCheck_ReturnsTrue_WhenValid()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         Assert.True(MatchAutomationChecks.PassesNameCheck(match));
//     }
//
//     // Games
//
//     // [Fact]
//     // public void Match_GameModeInvalid()
//     // {
//     //     Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//     //
//     //     match.Tournament.Ruleset = 5;
//     //     Assert.False(MatchAutomationChecks.ValidGameMode(match));
//     //
//     //     match.Tournament.Ruleset = -1;
//     //     Assert.False(MatchAutomationChecks.ValidGameMode(match));
//     // }
//
//     [Fact]
//     public void Match_Games_PassAutomatedChecks()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//
//         Assert.Multiple(() =>
//         {
//             foreach (Game game in match.Games)
//             {
//                 Assert.True(GameAutomationChecks.PassesAutomationChecks(game));
//             }
//         });
//     }
//
//     [Fact]
//     public void Match_Games_PassTeamSizeCheck()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//
//         Assert.Multiple(() =>
//         {
//             foreach (Game game in match.Games)
//             {
//                 Assert.True(GameAutomationChecks.PassesTeamSizeCheck(game));
//             }
//         });
//     }
//
//     [Fact]
//     public void Game_FailsTeamSizeCheck_WhenImbalancedRed()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match
//             .Games.First()
//             .MatchScores = new List<GameScore>
//         {
//             new()
//             {
//                 PlayerId = 1,
//                 Score = 500_000,
//                 Team = (int)Team.Red
//             },
//             new()
//             {
//                 PlayerId = 2,
//                 Score = 500_000,
//                 Team = (int)Team.Red
//             },
//             new()
//             {
//                 PlayerId = 3,
//                 Score = 500_000,
//                 Team = (int)Team.Blue
//             }
//         };
//
//         Assert.False(GameAutomationChecks.PassesTeamSizeCheck(match.Games.First()));
//     }
//
//     [Fact]
//     public void Game_FailsTeamSizeCheck_WhenImbalancedBlue()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match
//             .Games.First()
//             .MatchScores = new List<GameScore>
//         {
//             new()
//             {
//                 PlayerId = 1,
//                 Score = 500_000,
//                 Team = (int)Team.Blue
//             },
//             new()
//             {
//                 PlayerId = 2,
//                 Score = 500_000,
//                 Team = (int)Team.Blue
//             },
//             new()
//             {
//                 PlayerId = 3,
//                 Score = 500_000,
//                 Team = (int)Team.Red
//             }
//         };
//
//         Assert.False(GameAutomationChecks.PassesTeamSizeCheck(match.Games.First()));
//     }
//
//     [Fact]
//     public void Game_FailsTeamSizeCheck_WhenOneScoreIsZero()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Games.First().MatchScores = new List<GameScore>
//         {
//             new()
//             {
//                 PlayerId = 1,
//                 Score = 0,
//                 Team = (int)Team.Red
//             },
//             new()
//             {
//                 PlayerId = 2,
//                 Score = 250_000,
//                 Team = (int)Team.Red
//             },
//             new()
//             {
//                 PlayerId = 3,
//                 Score = 250_000,
//                 Team = (int)Team.Blue
//             },
//             new()
//             {
//                 PlayerId = 4,
//                 Score = 250_000,
//                 Team = (int)Team.Blue
//             }
//         };
//
//         Assert.False(GameAutomationChecks.PassesTeamSizeCheck(match.Games.First()));
//     }
//
//     [Fact]
//     public void Game_FailsTeamSizeCheck_WhenFair2v2_And_ThreeScoresAreZero()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Tournament.TeamSize = 2;
//
//         match.Games.First().MatchScores = new List<GameScore>
//         {
//             new()
//             {
//                 PlayerId = -1,
//                 Score = 0,
//                 Team = (int)Team.Red
//             },
//             new()
//             {
//                 PlayerId = -1,
//                 Score = 0,
//                 Team = (int)Team.Red
//             },
//             new()
//             {
//                 PlayerId = -1,
//                 Score = 0,
//                 Team = (int)Team.Blue
//             },
//             new()
//             {
//                 PlayerId = -1,
//                 Score = 250_000,
//                 Team = (int)Team.Blue
//             }
//         };
//
//         Assert.False(GameAutomationChecks.PassesTeamSizeCheck(match.Games.First()));
//     }
//
//     [Fact]
//     public void Game_FailsTeamSizeCheck_WhenDiffersFromTournamentSize()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Tournament.TeamSize = 4;
//
//         Assert.False(GameAutomationChecks.PassesTeamSizeCheck(match.Games.First()));
//     }
//
//     [Fact]
//     public void Match_Games_PassModeCheck()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//
//         Assert.Multiple(() =>
//         {
//             foreach (Game game in match.Games)
//             {
//                 Assert.True(GameAutomationChecks.PassesRulesetCheck(game));
//             }
//         });
//     }
//
//     // [Fact]
//     // public void Game_FailsModeCheck_WhenInvalidMode()
//     // {
//     //     Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//     //     match.Tournament.Ruleset = 5;
//     //
//     //     Assert.False(GameAutomationChecks.PassesRulesetCheck(match.Games.First()));
//     // }
//
//     [Fact]
//     public void Game_FailsModeCheck_WhenDiffersFromTournamentMode()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Tournament.Ruleset = Ruleset.Taiko;
//
//         Assert.False(GameAutomationChecks.PassesRulesetCheck(match.Games.First()));
//     }
//
//     [Fact]
//     public void Game_FailsScoringCheck_WhenComboScoring()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Games.First().ScoringType = ScoringType.Combo;
//
//         Assert.False(GameAutomationChecks.PassesScoringTypeCheck(match.Games.First()));
//     }
//
//     [Fact]
//     public void Game_FailsTeamTypeCheck_WhenTagTeamMode()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Games.First().TeamType = TeamType.TagTeamVs;
//
//         Assert.False(GameAutomationChecks.PassesTeamTypeCheck(match.Games.First()));
//
//         match.Games.First().TeamType = TeamType.TagCoop;
//         Assert.False(GameAutomationChecks.PassesTeamTypeCheck(match.Games.First()));
//     }
//
//     [Fact]
//     public void Game_FailsTeamTypeCheck_WhenHeadToHead_And_TournamentSizeNotOne()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Games.First().TeamType = TeamType.HeadToHead;
//         match.Tournament.TeamSize = 4;
//
//         Assert.False(GameAutomationChecks.PassesTeamTypeCheck(match.Games.First()));
//     }
//
//     [Fact]
//     public void Game_PassesTeamTypeCheck_WhenHeadToHead_And_TournamentSizeIsOne()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Games.First().TeamType = TeamType.HeadToHead;
//         match.Tournament.TeamSize = 1;
//
//         Assert.True(GameAutomationChecks.PassesTeamTypeCheck(match.Games.First()));
//     }
//
//     // [Fact]
//     // public void Game_FailsAutomationChecks_WhenAllPlayersSameTeam_AndTeamVs()
//     // {
//     // 	var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//     // 	match.Games.First().TeamType = (int)OsuEnums.TeamType.TeamVs;
//     // 	foreach(var score in match.Games.First().MatchScores)
//     // 	{
//     // 		score.Team = (int)OsuEnums.Team.Red;
//     // 	}
//     //
//     // 	Assert.False(GameAutomationChecks.PassesAutomationChecks(match.Games.First()));
//     // }
//
//     [Fact]
//     public void Game_FailsTeamSizeCheck_WhenInvalidTeamSizing()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Tournament.TeamSize = 0;
//         Assert.False(GameAutomationChecks.PassesTeamSizeCheck(match.Games.First()));
//
//         match.Tournament.TeamSize = 9;
//         Assert.False(GameAutomationChecks.PassesTeamSizeCheck(match.Games.First()));
//     }
//
//     [Fact]
//     public void Game_FailsTeamSizeCheck_WithAnyNoTeam()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//
//         match.Tournament.TeamSize = 2;
//
//         match
//             .Games.First()
//             .MatchScores = new List<GameScore>
//         {
//             new()
//             {
//                 PlayerId = 1,
//                 Score = 500_000,
//                 Team = (int)Team.NoTeam
//             },
//             new()
//             {
//                 PlayerId = 2,
//                 Score = 500_000,
//                 Team = (int)Team.Red
//             },
//             new()
//             {
//                 PlayerId = 3,
//                 Score = 500_000,
//                 Team = (int)Team.Blue
//             },
//             new()
//             {
//                 PlayerId = 4,
//                 Score = 500_000,
//                 Team = (int)Team.NoTeam
//             }
//         };
//
//         Assert.False(GameAutomationChecks.PassesTeamSizeCheck(match.Games.First()));
//     }
//
//     [Fact]
//     public void Game_FailsTeamSizeCheck_WhenFourNoTeam_AndTeamSizeTwo()
//     {
//         // Set the game's scores to: 4 players with NoTeam and team size 2
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Tournament.TeamSize = 2;
//
//         match.Games.First().MatchScores = new List<GameScore>
//         {
//             new()
//             {
//                 PlayerId = -1,
//                 Score = 500_000,
//                 Team = (int)Team.NoTeam
//             },
//             new()
//             {
//                 PlayerId = -1,
//                 Score = 500_000,
//                 Team = (int)Team.NoTeam
//             },
//             new()
//             {
//                 PlayerId = -1,
//                 Score = 500_000,
//                 Team = (int)Team.NoTeam
//             },
//             new()
//             {
//                 PlayerId = -1,
//                 Score = 500_000,
//                 Team = (int)Team.NoTeam
//             }
//         };
//
//         Assert.False(GameAutomationChecks.PassesTeamSizeCheck(match.Games.First()));
//     }
//
//     [Fact]
//     public void Match_Games_PassScoringTypeCheck()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//
//         Assert.Multiple(() =>
//         {
//             foreach (Game game in match.Games)
//             {
//                 Assert.True(GameAutomationChecks.PassesScoringTypeCheck(game));
//             }
//         });
//     }
//
//     [Fact]
//     public void Game_FailsScoringTypeCheck_WhenNotScoreV2()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         var badScoringTypes = new List<ScoringType>
//         {
//             ScoringType.Score,
//             ScoringType.Combo,
//             ScoringType.Accuracy
//         };
//
//         Assert.Multiple(() =>
//         {
//             foreach (ScoringType scoringType in badScoringTypes)
//             {
//                 match.Games.First().ScoringType = scoringType;
//                 Assert.False(GameAutomationChecks.PassesScoringTypeCheck(match.Games.First()));
//             }
//         });
//     }
//
//     [Fact]
//     public void Match_Games_PassModsCheck()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//
//         Assert.Multiple(() =>
//         {
//             foreach (Game game in match.Games)
//             {
//                 Assert.True(GameAutomationChecks.PassesModsCheck(game));
//             }
//         });
//     }
//
//     [Fact]
//     public void Match_Games_PassTeamTypeCheck()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//
//         Assert.Multiple(() =>
//         {
//             foreach (Game game in match.Games)
//             {
//                 Assert.True(GameAutomationChecks.PassesTeamTypeCheck(game));
//             }
//         });
//     }
//
//     [Fact]
//     public void Game_FailsTeamSizeCheck()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Tournament.TeamSize = 4;
//
//         Assert.False(GameAutomationChecks.PassesTeamSizeCheck(match.Games.First()));
//     }
//
//     [Fact]
//     public void Game_FailsTeamSizeCheck_Unbalanced_Teams()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//
//         match
//             .Games.First().MatchScores = new List<GameScore>
//         {
//             new()
//             {
//                 PlayerId = 0,
//                 Score = 100000,
//                 Team = (int)Team.Red,
//                 IsValid = true
//             },
//             new()
//             {
//                 PlayerId = 1,
//                 Score = 100000,
//                 Team = (int)Team.Red,
//                 IsValid = true
//             },
//             new()
//             {
//                 PlayerId = 2,
//                 Score = 100000,
//                 Team = (int)Team.Blue,
//                 IsValid = true
//             }
//         };
//
//         Assert.False(GameAutomationChecks.PassesTeamSizeCheck(match.Games.First()));
//     }
//
//     [Fact]
//     public void Game_FailsModsCheck()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Games.First().Mods = Mods.Relax;
//
//         Assert.False(GameAutomationChecks.PassesModsCheck(match.Games.First()));
//     }
//
//     [Fact]
//     public void Game_FailsModeCheck()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Games.First().Ruleset = Ruleset.Catch;
//
//         Assert.False(GameAutomationChecks.PassesRulesetCheck(match.Games.First()));
//     }
//
//     [Fact]
//     public void Game_FailsScoringTypeCheck()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Games.First().ScoringType = ScoringType.Combo;
//
//         Assert.False(GameAutomationChecks.PassesScoringTypeCheck(match.Games.First()));
//     }
//
//     [Fact]
//     public void Game_FailsTeamTypeCheck()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Games.First().TeamType = TeamType.TagCoop;
//
//         Assert.False(GameAutomationChecks.PassesTeamTypeCheck(match.Games.First()));
//
//         match.Games.First().TeamType = TeamType.TagTeamVs;
//         Assert.False(GameAutomationChecks.PassesTeamTypeCheck(match.Games.First()));
//     }
//
//     [Fact]
//     public void Game_PassesTeamTypeCheck_HeadToHead()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         Game game = match.Games.First();
//         game.TeamType = TeamType.HeadToHead;
//
//         game.Match.Tournament.TeamSize = 4;
//         Assert.False(GameAutomationChecks.PassesTeamTypeCheck(game));
//
//         game.Match.Tournament.TeamSize = 1;
//         Assert.True(GameAutomationChecks.PassesTeamTypeCheck(game));
//     }
//
//     [Fact]
//     public void Game_FailsTeamTypeCheck_HeadToHead_When_TeamSize_Is_2()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Games.First().TeamType = TeamType.HeadToHead;
//         match.Tournament.TeamSize = 2;
//
//         Assert.False(GameAutomationChecks.PassesTeamTypeCheck(match.Games.First()));
//     }
//
//     [Fact]
//     public void Game_PassesScoreSanity_WhenAnyScoresPresent()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         Game game = match.Games.First();
//
//         Assert.True(GameAutomationChecks.PassesScoreSanityCheck(game));
//     }
//
//     [Fact]
//     public void Game_FailsScoreSanity_WhenNoScores()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Games.First().MatchScores = new List<GameScore>();
//
//         Assert.False(GameAutomationChecks.PassesScoreSanityCheck(match.Games.First()));
//     }
//
//     [Fact]
//     public void Game_PassesSanity_WhenRefereeInLobby_TeamRed_1v1()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match
//             .Games.First()
//             .MatchScores = new List<GameScore>
//         {
//             new()
//             {
//                 PlayerId = 0,
//                 Score = 0,
//                 Team = (int)Team.Red,
//                 IsValid = false
//             },
//             new()
//             {
//                 PlayerId = 1,
//                 Score = 100503,
//                 Team = (int)Team.Red,
//                 IsValid = true
//             },
//             new()
//             {
//                 PlayerId = 2,
//                 Score = 100000,
//                 Team = (int)Team.Blue,
//                 IsValid = true
//             }
//         };
//
//         Assert.True(GameAutomationChecks.PassesScoreSanityCheck(match.Games.First()));
//         Assert.True(GameAutomationChecks.PassesTeamSizeCheck(match.Games.First()));
//         Assert.True(GameAutomationChecks.PassesAutomationChecks(match.Games.First()));
//     }
//
//     [Fact]
//     public void Game_PassesSanity_WhenRefereeInLobby_TeamBlue_1v1()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match
//             .Games.First()
//             .MatchScores = new List<GameScore>
//         {
//             new()
//             {
//                 PlayerId = 0,
//                 Score = 0,
//                 Team = (int)Team.Blue,
//                 IsValid = false
//             },
//             new()
//             {
//                 PlayerId = 1,
//                 Score = 100503,
//                 Team = (int)Team.Blue,
//                 IsValid = true
//             },
//             new()
//             {
//                 PlayerId = 2,
//                 Score = 100000,
//                 Team = (int)Team.Red,
//                 IsValid = true
//             }
//         };
//
//         Assert.True(GameAutomationChecks.PassesScoreSanityCheck(match.Games.First()));
//         Assert.True(GameAutomationChecks.PassesTeamSizeCheck(match.Games.First()));
//         Assert.True(GameAutomationChecks.PassesAutomationChecks(match.Games.First()));
//     }
//
//     [Fact]
//     public void Game_PassesChecks_WhenRefereeInLobby_TeamRed_2v2()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Tournament.TeamSize = 2;
//
//         match.Games.First().MatchScores = new List<GameScore>()
//         {
//             new() // Referee
//             {
//                 PlayerId = 1,
//                 Score = 0,
//                 Team = (int)Team.Red,
//                 IsValid = false
//             },
//             new()
//             {
//                 PlayerId = 2,
//                 Score = 100000,
//                 Team = (int)Team.Red,
//                 IsValid = true
//             },
//             new()
//             {
//                 PlayerId = 3,
//                 Score = 100000,
//                 Team = (int)Team.Red,
//                 IsValid = true
//             },
//             new()
//             {
//                 PlayerId = 4,
//                 Score = 100000,
//                 Team = (int)Team.Blue,
//                 IsValid = true
//             },
//             new()
//             {
//                 PlayerId = 5,
//                 Score = 100000,
//                 Team = (int)Team.Blue,
//                 IsValid = true
//             }
//         };
//
//         Assert.True(GameAutomationChecks.PassesTeamSizeCheck(match.Games.First()));
//         Assert.True(GameAutomationChecks.PassesAutomationChecks(match.Games.First()));
//     }
//
//     [Fact]
//     public void Game_PassesChecks_WhenRefereeInLobby_TeamBlue_2v2()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Tournament.TeamSize = 2;
//
//         match.Games.First().MatchScores = new List<GameScore>
//         {
//             new() // Referee
//             {
//                 PlayerId = 1,
//                 Score = 0,
//                 Team = (int)Team.Blue,
//                 IsValid = false
//             },
//             new()
//             {
//                 PlayerId = 2,
//                 Score = 100000,
//                 Team = (int)Team.Red,
//                 IsValid = true
//             },
//             new()
//             {
//                 PlayerId = 3,
//                 Score = 100000,
//                 Team = (int)Team.Red,
//                 IsValid = true
//             },
//             new()
//             {
//                 PlayerId = 4,
//                 Score = 100000,
//                 Team = (int)Team.Blue,
//                 IsValid = true
//             },
//             new()
//             {
//                 PlayerId = 5,
//                 Score = 100000,
//                 Team = (int)Team.Blue,
//                 IsValid = true
//             }
//         };
//
//         Assert.True(GameAutomationChecks.PassesTeamSizeCheck(match.Games.First()));
//         Assert.True(GameAutomationChecks.PassesAutomationChecks(match.Games.First()));
//     }
//
//     [Fact]
//     public void Game_FailsTeamSizeCheck_WhenOneZeroScore_CausesInvalid2v2()
//     {
//         // This test ensures that a player who earns 0 score is not
//         // accidentally flagged as a referee despite meeting a lot of the
//         // criteria
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Tournament.TeamSize = 2;
//
//         match.Games.First().MatchScores = new List<GameScore>
//         {
//             new() // Player on team red gets 0 score, NOT a referee
//             {
//                 PlayerId = 0,
//                 Score = 0,
//                 Team = (int)Team.Red
//             },
//             new()
//             {
//                 PlayerId = 0,
//                 Score = 100000,
//                 Team = (int)Team.Red
//             },
//             new()
//             {
//                 PlayerId = 0,
//                 Score = 100000,
//                 Team = (int)Team.Blue
//             },
//             new()
//             {
//                 PlayerId = 0,
//                 Score = 100000,
//                 Team = (int)Team.Blue
//             }
//         };
//
//         Assert.False(GameAutomationChecks.PassesTeamSizeCheck(match.Games.First()));
//     }
//
//     // Scores
//     [Fact]
//     public void Score_Invalid_WhenZeroScore()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         match.Games.First().MatchScores.First().Score = 0;
//
//         Assert.False(ScoreAutomationChecks.PassesAutomationChecks(match.Games.First().MatchScores.First()));
//     }
//
//     [Fact]
//     public void Scores_PassAutomationChecks()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//
//         Assert.Multiple(() =>
//         {
//             foreach (GameScore? score in match.Games.SelectMany(x => x.MatchScores))
//             {
//                 Assert.True(ScoreAutomationChecks.PassesAutomationChecks(score));
//             }
//         });
//     }
//
//     [Fact]
//     public void Scores_PassScoreRequirementCheck()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//
//         Assert.Multiple(() =>
//         {
//             foreach (GameScore? score in match.Games.SelectMany(x => x.MatchScores))
//             {
//                 Assert.True(ScoreAutomationChecks.PassesModsCheck(score));
//             }
//         });
//     }
//
//     [Fact]
//     public void Scores_FailScoreRequirementCheck()
//     {
//         Match match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
//         var forbiddenMods = new List<Mods>
//         {
//             Mods.Relax,
//             Mods.Relax2,
//             Mods.SpunOut,
//             Mods.SuddenDeath,
//             Mods.Perfect
//         };
//
//         Assert.Multiple(() =>
//         {
//             foreach (Mods mod in forbiddenMods)
//             {
//                 match.Games.First().MatchScores.First().EnabledMods = (int)mod;
//                 Assert.False(ScoreAutomationChecks.PassesModsCheck(match.Games.First().MatchScores.First()));
//             }
//         });
//     }
// }
