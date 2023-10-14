using API.Entities;
using API.Enums;
using API.Osu;
using API.Osu.AutomationChecks;
using API.Services.Interfaces;
using Moq;

namespace APITests.Osu;

public class MatchAutomationChecksTests
{
	private readonly Mock<IMatchesService> _matchesServiceMock = new();

	public MatchAutomationChecksTests()
	{
		var match = new API.Entities.Match
		{
			MatchId = 1,
			Abbreviation = "STT3",
			Created = DateTime.UtcNow,
			StartTime = new DateTime(2023, 1, 1, 0, 0,
				0),
			EndTime = new DateTime(2023, 1, 1, 1, 0,
				0),
			Forum = "https://osu.ppy.sh/community/forums/topics/1567938?n=1",
			Mode = 0,
			TournamentName = "Stage's Tranquility Tournament 3",
			RankRangeLowerBound = 10000,
			Name = "STT3: (the voices are back) vs (la planta)",
			TeamSize = 1,
			NeedsAutoCheck = true,
			IsApiProcessed = true,
			VerificationStatus = (int)MatchVerificationStatus.PendingVerification
		};

		var games = new List<Game>
		{
			new()
			{
				BeatmapId = 1,
				PlayMode = 0,
				StartTime = new DateTime(2023, 1, 1, 0, 0,
					0),
				EndTime = new DateTime(2023, 1, 1, 0, 1,
					0),
				GameId = 1,
				MatchId = 1,
				ScoringType = (int)OsuEnums.ScoringType.ScoreV2,
				TeamType = (int)OsuEnums.TeamType.TeamVs,
				Mods = (int)(OsuEnums.Mods.NoFail | OsuEnums.Mods.DoubleTime),
				Match = match
			}
		};

		var scores = new List<MatchScore>
		{
			new()
			{
				Id = 1,
				Count300 = 890,
				Count100 = 75,
				Count50 = 4,
				CountMiss = 3,
				MaxCombo = 620,
				Score = 489984,
				Team = (int)OsuEnums.Team.Red,
				Game = games.First()
			},
			new()
			{
				Id = 2,
				Count300 = 902,
				Count100 = 53,
				Count50 = 6,
				CountMiss = 11,
				MaxCombo = 545,
				Score = 400720,
				Team = (int)OsuEnums.Team.Blue,
				Game = games.First()
			}
		};

		games.First().MatchScores = scores;
		match.Games = games;

		_matchesServiceMock.Setup(x => x.GetMatchesNeedingAutoCheckAsync().Result)
		                   .Returns(new List<API.Entities.Match>
		                   {
			                   match
		                   });
	}

	[Fact]
	public async Task Mock_ReturnsCorrectObject()
	{
		var match = (await _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync()).First();

		Assert.NotNull(match);
		Assert.Equal("STT3: (the voices are back) vs (la planta)", match.Name);

		var firstGame = match.Games.First();
		Assert.NotNull(firstGame);
		Assert.Equal((int)(OsuEnums.Mods.NoFail | OsuEnums.Mods.DoubleTime), firstGame.Mods);

		var firstMatchScore = firstGame.MatchScores.First();
		Assert.NotNull(firstMatchScore);
		Assert.Equal(890, firstMatchScore.Count300);
		Assert.Equal(2, firstGame.MatchScores.Count);
	}

	
	// Matches

	/// <summary>
	///  Tests the flow of a match being processed for
	/// </summary>
	[Fact]
	public void Match_PassesAutomatedChecks()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		Assert.True(MatchAutomationChecks.PassesAllChecks(match));
	}

	[Fact]
	public void Match_Games_PassAutomatedChecks()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();

		Assert.Multiple(() =>
		{
			foreach (var game in match.Games)
			{
				Assert.True(GameAutomationChecks.PassesAutomationChecks(game));
			}
		});
	}

	[Fact]
	public void Match_Games_PassTeamSizeCheck()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();

		Assert.Multiple(() =>
		{
			foreach (var game in match.Games)
			{
				Assert.True(GameAutomationChecks.PassesTeamSizeCheck(game));
			}
		});
	}

	[Fact]
	public void Match_Games_PassModeCheck()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();

		Assert.Multiple(() =>
		{
			foreach (var game in match.Games)
			{
				Assert.True(GameAutomationChecks.PassesModeCheck(game));
			}
		});
	}

	[Fact]
	public void Match_Games_PassScoringTypeCheck()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();

		Assert.Multiple(() =>
		{
			foreach (var game in match.Games)
			{
				Assert.True(GameAutomationChecks.PassesScoringTypeCheck(game));
			}
		});
	}

	[Fact]
	public void Match_Games_PassModsCheck()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();

		Assert.Multiple(() =>
		{
			foreach (var game in match.Games)
			{
				Assert.True(GameAutomationChecks.PassesModsCheck(game));
			}
		});
	}

	[Fact]
	public void Match_Games_PassTeamTypeCheck()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();

		Assert.Multiple(() =>
		{
			foreach (var game in match.Games)
			{
				Assert.True(GameAutomationChecks.PassesTeamTypeCheck(game));
			}
		});
	}

	[Fact]
	public void Match_FailsNameCheck()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		match.Name = "STT4: (the voices are back) vs (la planta)";
		Assert.False(MatchAutomationChecks.PassesNameCheck(match));
	}

	[Fact]
	public void Match_FailsNameCheck_WithNullAbbreviation()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		match.Abbreviation = null;
		Assert.False(MatchAutomationChecks.PassesNameCheck(match));
	}

	[Fact]
	public void Match_FailsNameCheck_WithEmptyAbbreviation()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		match.Abbreviation = string.Empty;
		Assert.False(MatchAutomationChecks.PassesNameCheck(match));
	}

	[Fact]
	public void Match_FailsNameCheck_WithNullName()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		match.Name = null;
		Assert.False(MatchAutomationChecks.PassesNameCheck(match));
	}

	[Fact]
	public void Match_FailsNameCheck_WithEmptyName()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		match.Name = string.Empty;
		Assert.False(MatchAutomationChecks.PassesNameCheck(match));
	}
	
	// Games

	[Fact]
	public void Game_FailsTeamSizeCheck()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		match.TeamSize = 4;

		Assert.False(GameAutomationChecks.PassesTeamSizeCheck(match.Games.First()));
	}

	[Fact]
	public void Game_FailsTeamSizeCheck_Unbalanced_Teams()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		match.Games.First()!.MatchScores.Add(new MatchScore()
		{
			PlayerId = -1,
			Score = 500,
			Team = (int) OsuEnums.Team.Red
		});
		
		Assert.False(GameAutomationChecks.PassesTeamSizeCheck(match.Games.First()));
	}

	[Fact]
	public void Game_FailsModsCheck()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		match.Games.First().Mods = (int)OsuEnums.Mods.Relax;

		Assert.False(GameAutomationChecks.PassesModsCheck(match.Games.First()));
	}
	
	[Fact]
	public void Game_FailsModeCheck()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		match.Games.First().PlayMode = (int)OsuEnums.Mode.Catch;

		Assert.False(GameAutomationChecks.PassesModeCheck(match.Games.First()));
	}
	
	[Fact]
	public void Game_FailsScoringTypeCheck()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		match.Games.First().ScoringType = (int)OsuEnums.ScoringType.Combo;

		Assert.False(GameAutomationChecks.PassesScoringTypeCheck(match.Games.First()));
	}
	
	[Fact]
	public void Game_FailsTeamTypeCheck()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		match.Games.First().TeamType = (int)OsuEnums.TeamType.TagCoop;

		Assert.False(GameAutomationChecks.PassesTeamTypeCheck(match.Games.First()));
		
		match.Games.First().TeamType = (int)OsuEnums.TeamType.TagTeamVs;
		Assert.False(GameAutomationChecks.PassesTeamTypeCheck(match.Games.First()));
	}

	[Fact]
	public void Game_PassesTeamTypeCheck_HeadToHead()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		match.Games.First().TeamType = (int)OsuEnums.TeamType.HeadToHead;

		Assert.True(GameAutomationChecks.PassesTeamTypeCheck(match.Games.First()));
	}

	[Fact]
	public void Game_FailsTeamTypeCheck_HeadToHead_When_TeamSize_Is_2()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		match.Games.First().TeamType = (int)OsuEnums.TeamType.HeadToHead;
		match.TeamSize = 2;

		Assert.False(GameAutomationChecks.PassesTeamTypeCheck(match.Games.First()));
	}
	
	// Scores
	[Fact]
	public void Scores_PassAutomationChecks()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();

		Assert.Multiple(() =>
		{
			foreach (var score in match.Games.SelectMany(x => x.MatchScores))
			{
				Assert.True(ScoreAutomationChecks.PassesAutomationChecks(score));
			}
		});
	}

	[Fact]
	public void Scores_PassScoreRequirementCheck()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();

		Assert.Multiple(() =>
		{
			foreach (var score in match.Games.SelectMany(x => x.MatchScores))
			{
				Assert.True(ScoreAutomationChecks.PassesModsCheck(score));
			}
		});
	}
}