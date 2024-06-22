using Database.Entities;
using Database.Enums;

namespace API.Osu.AutomationChecks;

public static class MatchAutomationChecks
{
    private const string LogPrefix = "[Automations: Match Check]";
    private static readonly Serilog.ILogger s_logger = Serilog.Log.ForContext(typeof(MatchAutomationChecks));

    public static bool PassesAllChecks(Match match)
    {
        return HasTournament(match) && ValidGameMode(match) && PassesNameCheck(match);
    }

    public static bool HasTournament(Match match)
    {
        var passes = match.TournamentId == match.Tournament.Id;
        if (!passes)
        {
            s_logger.Information(
                "{Prefix} Match {MatchID} has no tournament, failing automation checks",
                LogPrefix,
                match.OsuId
            );
        }

        return passes;
    }

    public static bool PassesNameCheck(Match match)
    {
        if (string.IsNullOrEmpty(match.Tournament.Abbreviation))
        {
            s_logger.Information(
                "{Prefix} Match {MatchID} had null or empty abbreviation, failing",
                LogPrefix,
                match.OsuId
            );
            return false;
        }

        if (string.IsNullOrEmpty(match.Name))
        {
            s_logger.Information(
                "{Prefix} Match {MatchID} had null or empty name, failing",
                LogPrefix,
                match.OsuId
            );
            return false;
        }

        if (!match.Name.StartsWith(match.Tournament.Abbreviation, StringComparison.OrdinalIgnoreCase))
        {
            s_logger.Information(
                "{Prefix} Match {MatchID} had a name that didn't start with the expected abbreviation, failing",
                LogPrefix,
                match.OsuId
            );
            return false;
        }

        if (!LobbyNameChecker.IsNameValid(match.Name, match.Tournament.Abbreviation))
        {
            s_logger.Information(
                "{Prefix} Match {MatchID} had a name that didn't pass the lobby name check, failing",
                LogPrefix,
                match.OsuId
            );
            return false;
        }

        return true;
    }

    public static bool ValidGameMode(Match match)
    {
        // Ensures the mode for the match's tournament is valid.
        var valid = match.Tournament.Ruleset is >= Ruleset.Standard and <= Ruleset.Mania;

        if (!valid)
        {
            s_logger.Information(
                "{Prefix} Match {MatchID} had an invalid mode, failing automation checks",
                LogPrefix,
                match.OsuId
            );
        }

        return valid;
    }
}
