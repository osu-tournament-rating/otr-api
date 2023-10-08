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

	[SetUp]
	public void Setup()
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

	[Test]
	public void Mock_ReturnsCorrectObject() => Assert.Multiple(() =>
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();

		Assert.That(match, Is.Not.Null);
		Assert.That(match.Name, Is.EqualTo("STT3: (the voices are back) vs (la planta)"));

		Assert.That(match.Games.First(), Is.Not.Null);
		Assert.That(match.Games.First().Mods, Is.EqualTo((int)(OsuEnums.Mods.NoFail | OsuEnums.Mods.DoubleTime)));

		Assert.That(match.Games.First().MatchScores, Is.Not.Null);
		Assert.That(match.Games.First().MatchScores.First().Count300, Is.EqualTo(890));
		Assert.That(match.Games.First().MatchScores.Count, Is.EqualTo(2));
	});

	/// <summary>
	///  Tests the flow of a match being processed for
	/// </summary>
	[Test]
	public void Match_PassesAutomatedChecks()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		Assert.That(MatchAutomationChecks.PassesAllChecks(match), Is.True);
	}

	[Test]
	public void Match_Games_PassAutomatedChecks()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();

		Assert.Multiple(() =>
		{
			foreach (var game in match.Games)
			{
				Assert.That(GameAutomationChecks.PassesAutomationChecks(game), Is.True);
			}
		});
	}

	[Test]
	public void Match_Games_PassTeamSizeCheck()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();

		Assert.Multiple(() =>
		{
			foreach (var game in match.Games)
			{
				Assert.That(GameAutomationChecks.PassesTeamSizeCheck(game), Is.True);
			}
		});
	}

	[Test]
	public void Match_Games_PassModeCheck()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();

		Assert.Multiple(() =>
		{
			foreach (var game in match.Games)
			{
				Assert.That(GameAutomationChecks.PassesModeCheck(game), Is.True);
			}
		});
	}

	[Test]
	public void Match_Games_PassScoringTypeCheck()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();

		Assert.Multiple(() =>
		{
			foreach (var game in match.Games)
			{
				Assert.That(GameAutomationChecks.PassesScoringTypeCheck(game), Is.True);
			}
		});
	}

	[Test]
	public void Match_Games_PassModsCheck()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();

		Assert.Multiple(() =>
		{
			foreach (var game in match.Games)
			{
				Assert.That(GameAutomationChecks.PassesModsCheck(game), Is.True);
			}
		});
	}

	[Test]
	public void Match_Games_PassTeamTypeCheck()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();

		Assert.Multiple(() =>
		{
			foreach (var game in match.Games)
			{
				Assert.That(GameAutomationChecks.PassesTeamTypeCheck(game), Is.True);
			}
		});
	}

	[Test]
	public void Match_FailsNameCheck()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		match.Name = "STT4: (the voices are back) vs (la planta)";
		Assert.That(MatchAutomationChecks.PassesNameCheck(match), Is.False);
	}

	[Test]
	public void Match_FailsNameCheck_WithNullAbbreviation()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		match.Abbreviation = null;
		Assert.That(MatchAutomationChecks.PassesNameCheck(match), Is.False);
	}

	[Test]
	public void Match_FailsNameCheck_WithEmptyAbbreviation()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		match.Abbreviation = string.Empty;
		Assert.That(MatchAutomationChecks.PassesNameCheck(match), Is.False);
	}

	[Test]
	public void Match_FailsNameCheck_WithNullName()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		match.Name = null;
		Assert.That(MatchAutomationChecks.PassesNameCheck(match), Is.False);
	}

	[Test]
	public void Match_FailsNameCheck_WithEmptyName()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		match.Name = string.Empty;
		Assert.That(MatchAutomationChecks.PassesNameCheck(match), Is.False);
	}

	[Test]
	public void Game_FailsTeamSizeCheck()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		match.TeamSize = 2;

		Assert.That(GameAutomationChecks.PassesTeamSizeCheck(match.Games.First()), Is.False);
	}

	[Test]
	public void Game_FailsModsCheck()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		match.Games.First().Mods = (int)OsuEnums.Mods.Relax;

		Assert.That(GameAutomationChecks.PassesModsCheck(match.Games.First()), Is.False);
	}
	
	[Test]
	public void Game_FailsModeCheck()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		match.Games.First().PlayMode = (int)OsuEnums.Mode.Catch;

		Assert.That(GameAutomationChecks.PassesModeCheck(match.Games.First()), Is.False);
	}
	
	[Test]
	public void Game_FailsScoringTypeCheck()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		match.Games.First().ScoringType = (int)OsuEnums.ScoringType.Combo;

		Assert.That(GameAutomationChecks.PassesScoringTypeCheck(match.Games.First()), Is.False);
	}
	
	[Test]
	public void Game_FailsTeamTypeCheck()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		match.Games.First().TeamType = (int)OsuEnums.TeamType.TagCoop;

		Assert.That(GameAutomationChecks.PassesTeamTypeCheck(match.Games.First()), Is.False);
		
		match.Games.First().TeamType = (int)OsuEnums.TeamType.TagTeamVs;
		Assert.That(GameAutomationChecks.PassesTeamTypeCheck(match.Games.First()), Is.False);
	}

	[Test]
	public void Game_PassesTeamTypeCheck_HeadToHead()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		match.Games.First().TeamType = (int)OsuEnums.TeamType.HeadToHead;

		Assert.That(GameAutomationChecks.PassesTeamTypeCheck(match.Games.First()), Is.True);
	}

	[Test]
	public void Game_FailsTeamTypeCheck_HeadToHead_When_TeamSize_Is_2()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();
		match.Games.First().TeamType = (int)OsuEnums.TeamType.HeadToHead;
		match.TeamSize = 2;

		Assert.That(GameAutomationChecks.PassesTeamTypeCheck(match.Games.First()), Is.False);
	}
	
	// Scores
	[Test]
	public void Scores_PassAutomationChecks()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();

		Assert.Multiple(() =>
		{
			foreach (var score in match.Games.SelectMany(x => x.MatchScores))
			{
				Assert.That(ScoreAutomationChecks.PassesAutomationChecks(score), Is.True);
			}
		});
	}

	[Test]
	public void Scores_PassScoreRequirementCheck()
	{
		var match = _matchesServiceMock.Object.GetMatchesNeedingAutoCheckAsync().Result.First();

		Assert.Multiple(() =>
		{
			foreach (var score in match.Games.SelectMany(x => x.MatchScores))
			{
				Assert.That(ScoreAutomationChecks.PassesModsCheck(score), Is.True);
			}
		});
	}
}