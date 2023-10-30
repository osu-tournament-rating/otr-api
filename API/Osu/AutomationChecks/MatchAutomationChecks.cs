using API.Entities;

namespace API.Osu.AutomationChecks;

public static class MatchAutomationChecks
{
	private const string _logPrefix = "[Automations: Match Check]";
	private static readonly Serilog.ILogger _logger = Serilog.Log.ForContext(typeof(MatchAutomationChecks));
	
	public static bool PassesAllChecks(Match match)
	{
		return HasTournament(match) && ValidGameMode(match) && PassesNameCheck(match);
	}

	public static bool HasTournament(Match match)
	{
		bool passes = match.Tournament != null;
		if (!passes)
		{
			_logger.Warning("{Prefix} Match {MatchID} has no tournament, failing automation checks", _logPrefix, match.MatchId);
		}

		return passes;
	}

	public static bool PassesNameCheck(Match match)
	{
		if (string.IsNullOrEmpty(match.Tournament!.Abbreviation))
		{
			_logger.Information("{Prefix} Match {MatchID} had null or empty abbreviation, failing", _logPrefix, match.MatchId);
			return false;
		}

		if (string.IsNullOrEmpty(match.Name))
		{
			_logger.Information("{Prefix} Match {MatchID} had null or empty name, failing", _logPrefix, match.MatchId);
			return false;
		}

		if (!match.Name.StartsWith(match.Tournament!.Abbreviation, StringComparison.OrdinalIgnoreCase))
		{
			_logger.Information("{Prefix} Match {MatchID} had a name that didn't start with the expected abbreviation, failing", _logPrefix, match.MatchId);
			return false;
		}

		if (!LobbyNameChecker.IsNameValid(match.Name, match.Tournament!.Abbreviation))
		{
			_logger.Information("{Prefix} Match {MatchID} had a name that didn't pass the lobby name check, failing", _logPrefix, match.MatchId);
			return false;
		}

		return true;
	}

	public static bool ValidGameMode(Match match)
	{
		// Ensures the mode for the match's tournament is valid.
		bool valid = match.Tournament!.Mode is >= 0 and <= 3;

		if (!valid)
		{
			_logger.Information("{Prefix} Match {MatchID} had an invalid mode, failing automation checks", _logPrefix, match.MatchId);
		}

		return valid;
	}
}