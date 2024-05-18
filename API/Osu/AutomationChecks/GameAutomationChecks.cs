using API.Entities;
using API.Enums;
using API.Osu.Enums;
using API.Utilities;

namespace API.Osu.AutomationChecks;

public static class GameAutomationChecks
{
    private const string LogPrefix = "[Automations: Game Check]";
    private static readonly Serilog.ILogger s_logger = Serilog.Log.ForContext(typeof(GameAutomationChecks));

    public static bool PassesAutomationChecks(Game game) =>
        PassesScoringTypeCheck(game)
        && PassesRulesetCheck(game)
        && PassesTeamTypeCheck(game)
        && PassesTeamSizeCheck(game)
        && PassesModsCheck(game)
        && PassesScoreSanityCheck(game);

    /// <summary>
    /// Returns a <see cref="GameRejectionReason"/> which explains why a game
    /// is rejected.
    /// </summary>
    /// <param name="game">A game containing match scores</param>
    /// <returns>Null if the game passes automation checks, otherwise
    /// a <see cref="GameRejectionReason"/></returns>
    public static GameRejectionReason? IdentifyRejectionReason(Game game)
    {
        if (PassesAutomationChecks(game))
        {
            return null;
        }

        GameRejectionReason reason = 0;
        if (!PassesScoringTypeCheck(game))
        {
            reason |= GameRejectionReason.InvalidScoringType;
        }
        if (!PassesRulesetCheck(game))
        {
            reason |= GameRejectionReason.InvalidRuleset;
        }
        if (!PassesTeamTypeCheck(game))
        {
            reason |= GameRejectionReason.InvalidTeamType;
        }
        if (!PassesTeamSizeCheck(game))
        {
            reason |= GameRejectionReason.TeamSizeMismatch;
        }
        if (!PassesModsCheck(game))
        {
            reason |= GameRejectionReason.InvalidMods;
        }
        if (!PassesScoreSanityCheck(game))
        {
            reason |= GameRejectionReason.NoScores;
        }

        return reason;
    }

    public static bool PassesTeamSizeCheck(Game game)
    {
        Tournament tournament = game.Match.Tournament;

        int? teamSize = tournament.TeamSize;
        if (teamSize is < 1 or > 8)
        {
            s_logger.Information(
                "{Prefix} Tournament {TournamentId} has an invalid team size: {Size}, can't verify game {GameId}",
                LogPrefix,
                tournament.Id,
                tournament.TeamSize,
                game.GameId
            );

            return false;
        }

        var teamVs = game.TeamType == TeamType.TeamVs;
        if (teamSize == 1 && !teamVs)
        {
            var countPlayers = game.MatchScores.Count;
            var refereePresent = game.MatchScores.Any(score => score.Score <= AutomationChecksUtils.MinimumScore);

            var countAfterReferees = countPlayers - game.MatchScores.Count(score => score.Score <= AutomationChecksUtils.MinimumScore);

            var satisfiesOneVersusOne = refereePresent ? countAfterReferees == 2 : countPlayers == 2;
            if (!satisfiesOneVersusOne)
            {
                s_logger.Information(
                    "{Prefix} Match {MatchId} has a team size of 1, but does not satisfy the 1v1 checks, can't verify game {GameId} [had {CountPlayers} players | Referee: {Ref}]",
                    LogPrefix,
                    game.Match.MatchId,
                    game.GameId,
                    countPlayers,
                    refereePresent
                );
            }

            return satisfiesOneVersusOne;
        }

        // teamSize is greater than 1 or is TeamVS

        var countRed = game.MatchScores.Count(s => s is { Team: (int)Team.Red, Score: > AutomationChecksUtils.MinimumScore });
        var countBlue = game.MatchScores.Count(s => s is { Team: (int)Team.Blue, Score: > AutomationChecksUtils.MinimumScore });
        var countNoTeam = game.MatchScores.Count(s => s.Team == (int)Team.NoTeam);

        if (countNoTeam > 0)
        {
            s_logger.Information(
                "{Prefix} Match {MatchId} has scores with no team but is in TeamVS mode, can't verify game {GameId}",
                LogPrefix,
                game.Match.MatchId,
                game.GameId
            );

            return false;
        }

        if (countRed == 0 || countBlue == 0)
        {
            // Situation where either no scores or valid, or there are only scores from one team, something isn't right.
            s_logger.Information(
                "{Prefix} Match {MatchId} has no team size for red or blue, can't verify game {GameId} (likely a warmup)",
                LogPrefix,
                game.Match.MatchId,
                game.GameId
            );

            return false;
        }

        var hasReferee = false;
        if (countRed != countBlue)
        {
            /*
             * Requirements for referee to be valid and present:
             *
             * - After referees are filtered out, the teams are perfectly even.
             */
            var refCountRed = game.MatchScores.Count(s => s is { Team: (int)Team.Red, Score: <= AutomationChecksUtils.MinimumScore });
            var refCountBlue = game.MatchScores.Count(s => s is { Team: (int)Team.Blue, Score: <= AutomationChecksUtils.MinimumScore });

            hasReferee = (countRed - refCountRed) == (countBlue - refCountBlue);

            if (!hasReferee)
            {
                s_logger.Information(
                    "{Prefix} Match {MatchId} has a mismatched team size: [Red: {Red} | Blue: {Blue}], can't verify game {GameId}",
                    LogPrefix,
                    game.Match.MatchId,
                    countRed,
                    countBlue,
                    game.GameId
                );

                return false;
            }
        }

        if (!IsMismatchedTeamSize(countRed, countBlue, tournament.TeamSize, hasReferee))
        {
            return true;
        }

        s_logger.Information(
            "{Prefix} Match {MatchId} has an unexpected team configuration: [Expected: {Expected}] [Red: {Red} | Blue: {Blue}], can't verify game {GameId} (Referee present: {HasReferee})",
            LogPrefix,
            game.Match.MatchId,
            tournament.TeamSize,
            countRed,
            countBlue,
            game.GameId,
            hasReferee
        );

        return false;
    }

    private static bool IsMismatchedTeamSize(int red, int blue, int expectedSize, bool hasReferee)
    {
        // If the ref is present, the team sizes can be off by exactly one.
        if (hasReferee)
        {
            // Should always equal 1 if the ref is present.
            // If not, the team sizes are definitely mismatched.
            return Math.Abs(red - blue) != 1;
        }

        var redUnexpected = red != expectedSize;
        var blueUnexpected = blue != expectedSize;

        return redUnexpected || blueUnexpected;
    }

    public static bool PassesRulesetCheck(Game game)
    {
        Tournament tournament = game.Match.Tournament;
        var tournamentRuleset = (Ruleset)tournament.Mode;

        if (!Enum.GetValues<Ruleset>().Contains(tournamentRuleset))
        {
            s_logger.Information(
                "{Prefix} Tournament {TournamentId} has an invalid ruleset: {Mode}, can't verify game {GameId}",
                LogPrefix,
                tournament.Id,
                tournament.Mode,
                game.GameId
            );

            return false;
        }

        if (tournamentRuleset == game.Ruleset)
        {
            return true;
        }

        s_logger.Information(
            "{Prefix} Tournament {TournamentId} has a game mode that differs from game, can't verify game {GameId} [Tournament: Mode={TMode} | Game: Mode={GMode}]",
            LogPrefix,
            tournament.Id,
            game.GameId,
            (Ruleset)tournament.Mode,
            game.Ruleset
        );

        return false;
    }

    public static bool PassesScoringTypeCheck(Game game)
    {
        if (game.ScoringType == ScoringType.ScoreV2)
        {
            return true;
        }

        s_logger.Information(
            "{Prefix} Match {MatchId} does not have a ScoreV2 scoring type, can't verify game {GameId}",
            LogPrefix,
            game.Match.MatchId,
            game.GameId
        );
        return false;

    }

    public static bool PassesModsCheck(Game game)
    {
        // If the game features invalid mods, it fails the check
        if (AutomationConstants.UnallowedMods.Any(unallowedMod => game.Mods.HasFlag(unallowedMod)))
        {
            return false;
        }

        // If any of the scores feature invalid mods, the check fails as well.
        foreach (MatchScore score in game.MatchScores)
        {
            if (score.EnabledModsEnum is null)
            {
                continue;
            }

            if (AutomationConstants.UnallowedMods.Any(unallowedMod =>
                    score.EnabledModsEnum.Value.HasFlag(unallowedMod)))
            {
                return false;
            }
        }

        return true;
    }

    public static bool PassesTeamTypeCheck(Game game)
    {
        switch (game.TeamType)
        {
            case TeamType.TagTeamVs or TeamType.TagCoop:
                s_logger.Information(
                    "{Prefix} Match {MatchId} has a tag team type, can't verify game {GameId}",
                    LogPrefix,
                    game.Match.MatchId,
                    game.GameId
                );
                return false;
            case TeamType.TeamVs:
                return true;
        }

        if (game.TeamType is TeamType.HeadToHead && game.Match.Tournament.TeamSize == 1)
        {
            return true;
        }

        s_logger.Information(
            "{Prefix} Match {MatchId} has a HeadToHead team type, but team size is not 1, can't verify game {GameId}",
            LogPrefix,
            game.Match.MatchId,
            game.GameId
        );

        return false;
    }

    public static bool PassesScoreSanityCheck(Game game)
    {
        if (game.MatchScores.Count != 0)
        {
            return true;
        }

        s_logger.Warning("Game {GameId} has no scores, can't verify", game.GameId);
        return false;
    }
}
