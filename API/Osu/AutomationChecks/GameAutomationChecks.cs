using API.Entities;

namespace API.Osu.AutomationChecks;

public static class GameAutomationChecks
{
	private const string _logPrefix = "[Automations: Game Check]";
	private static readonly Serilog.ILogger _logger = Serilog.Log.ForContext(typeof(GameAutomationChecks));
	
	public static bool PassesAutomationChecks(Game game)
	{
		return PassesScoringTypeCheck(game) && PassesModeCheck(game) && PassesTeamTypeCheck(game) && PassesTeamSizeCheck(game) && PassesModsCheck(game);
	}

	public static bool PassesTeamSizeCheck(Game game)
	{
		int? teamSize = game.Match.TeamSize;
		if (teamSize == null)
		{
			_logger.Information("{Prefix} Match {MatchId} has no team size, can't verify game {GameId}", _logPrefix, game.Match.Id, game.GameId);
			return false;
		}

		if (teamSize is < 1 or > 8)
		{
			_logger.Information("{Prefix} Match {MatchId} has an invalid team size: {Size}, can't verify game {GameId}", _logPrefix, game.Match.Id, game.Match.TeamSize, game.GameId);
			return false;
		}
		
		int countRed = game.MatchScores.Count(s => s.Team == (int)OsuEnums.Team.Red);
		int countBlue = game.MatchScores.Count(s => s.Team == (int)OsuEnums.Team.Blue);

		if (countRed == 0 && countBlue == 0)
		{
			// We likely have a situation where the team size is > 0, and the game is TeamVs,
			// but the match itself is HeadToHead. This is a problem.
			_logger.Information("{Prefix} Match {MatchId} has no team size for red or blue, can't verify game {GameId} (likely a warmup)", _logPrefix, game.Match.Id, game.GameId);
			return false;
		}
		
		if(countRed != teamSize || countBlue != teamSize)
		{
			_logger.Information("{Prefix} Match {MatchId} has an imbalanced team size: [Red: {Red} | Blue: {Blue}], can't verify game {GameId}", _logPrefix, game.Match.Id, countRed, countBlue, game.GameId);
			return false;
		}

		return true;
	}
	
	public static bool PassesModeCheck(Game game)
	{
		int? gameMode = game.Match.Mode;
		if (gameMode == null)
		{
			_logger.Information("{Prefix} Match {MatchId} has no game mode, can't verify game {GameId}", _logPrefix, game.Match.Id, game.GameId);
			return false;
		}
		
		if (gameMode is < 0 or > 3)
		{
			_logger.Information("{Prefix} Match {MatchId} has an invalid game mode: {Mode}, can't verify game {GameId}", _logPrefix, game.Match.Id, game.Match.Mode, game.GameId);
			return false;
		}
		
		if (gameMode != game.PlayMode)
		{
			_logger.Information("{Prefix} Match {MatchId} has a mismatched game mode: {Mode}, can't verify game {GameId}", _logPrefix, game.Match.Id, game.Match.Mode, game.GameId);
			return false;
		}

		return true;
	}
	
	public static bool PassesScoringTypeCheck(Game game)
	{
		if (game.ScoringType == (int)OsuEnums.ScoringType.Combo)
		{
			_logger.Information("{Prefix} Match {MatchId} has a combo scoring type, can't verify game {GameId}", _logPrefix, game.Match.Id, game.GameId);
			return false;
		}
		
		return true;
	}
	
	public static bool PassesModsCheck(Game game)
	{
		return !AutomationConstants.UnallowedMods.Any(unallowedMod => game.ModsEnum.HasFlag(unallowedMod));
	}
	
	public static bool PassesTeamTypeCheck(Game game)
	{
		var teamType = game.TeamTypeEnum;
		if (teamType is OsuEnums.TeamType.TagTeamVs or OsuEnums.TeamType.TagCoop)
		{
			_logger.Information("{Prefix} Match {MatchId} has a tag team type, can't verify game {GameId}", _logPrefix, game.Match.Id, game.GameId);
			return false;
		}
		
		// Ensure team size is valid
		if (teamType == OsuEnums.TeamType.HeadToHead)
		{
			if (game.Match.TeamSize != 1)
			{
				_logger.Information("{Prefix} Match {MatchId} has a HeadToHead team type, but team size is not 1, can't verify game {GameId}", _logPrefix, game.Match.Id, game.GameId);
				return false;
			}
			
			return true;
		}
		
		// TeamVs can be used for any team size
		return true;
	}
}