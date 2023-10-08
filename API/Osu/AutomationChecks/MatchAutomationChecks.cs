using API.Entities;

namespace API.Osu.AutomationChecks;

public static class MatchAutomationChecks
{
	private const string _logPrefix = "[Automations: Match Check]";
	private static readonly Serilog.ILogger _logger = Serilog.Log.ForContext(typeof(MatchAutomationChecks));
	
	public static bool PassesAllChecks(Match match)
	{
		return PassesNameCheck(match);
	}

	public static bool PassesNameCheck(Match match)
	{
		if (string.IsNullOrEmpty(match.Abbreviation))
		{
			_logger.Information("{Prefix} Match {MatchID} had null or empty abbreviation, failing", _logPrefix, match.MatchId);
			return false;
		}

		if (string.IsNullOrEmpty(match.Name))
		{
			_logger.Information("{Prefix} Match {MatchID} had null or empty name, failing", _logPrefix, match.MatchId);
			return false;
		}

		if (!match.Name.StartsWith(match.Abbreviation, StringComparison.OrdinalIgnoreCase))
		{
			_logger.Information("{Prefix} Match {MatchID} had a name that didn't start with the expected abbreviation, failing", _logPrefix, match.MatchId);
			return false;
		}

		if (!LobbyNameChecker.IsNameValid(match.Name, match.Abbreviation))
		{
			_logger.Information("{Prefix} Match {MatchID} had a name that didn't pass the lobby name check, failing", _logPrefix, match.MatchId);
			return false;
		}

		return true;
	}
}