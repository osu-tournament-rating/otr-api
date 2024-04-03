using API.Entities;
using API.Utilities;

namespace API.Osu.AutomationChecks;

public static class GameAutomationChecks
{
    private const string LogPrefix = "[Automations: Game Check]";
    private static readonly Serilog.ILogger s_logger = Serilog.Log.ForContext(typeof(GameAutomationChecks));

    public static bool PassesAutomationChecks(Game game) =>
        PassesScoringTypeCheck(game)
        && PassesModeCheck(game)
        && PassesTeamTypeCheck(game)
        && PassesTeamSizeCheck(game)
        && PassesModsCheck(game)
        && PassesScoreSanityCheck(game);

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

        if (teamSize == 1)
        {
            var countPlayers = game.MatchScores.Count;
            var refereePresent = game.MatchScores.Any(score => score.Score == 0);
            var satisfiesOneVersusOne = refereePresent ? countPlayers == 3 : countPlayers == 2;
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

        var countRed = game.MatchScores.Count(s => s is { Team: (int)OsuEnums.Team.Red, Score: > AutomationChecksUtils.MINIMUM_SCORE });
        var countBlue = game.MatchScores.Count(s => s is { Team: (int)OsuEnums.Team.Blue, Score: > AutomationChecksUtils.MINIMUM_SCORE });

        if (countRed == 0 && countBlue == 0)
        {
            // We likely have a situation where the team size is > 0, and the game is TeamVs,
            // but the match itself is HeadToHead. This is a problem.
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
             * - Exactly 1 score is below the minimum
             * - The team sizes are off by exactly 1
             */
            hasReferee = game.MatchScores.Count(s => s.Score <= AutomationChecksUtils.MINIMUM_SCORE) == 1 &&
                          Math.Abs(countRed - countBlue) == 1;
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

        if (IsMismatchedTeamSize(countRed, countBlue, tournament.TeamSize, hasReferee))
        {
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

        return true;
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

    public static bool PassesModeCheck(Game game)
    {
        Tournament tournament = game.Match.Tournament;
        var gameMode = tournament.Mode;

        if (gameMode is < 0 or > 3)
        {
            s_logger.Information(
                "{Prefix} Tournament {TournamentId} has an invalid game mode: {Mode}, can't verify game {GameId}",
                LogPrefix,
                tournament.Id,
                tournament.Mode,
                game.GameId
            );

            return false;
        }

        if (gameMode != game.PlayMode)
        {
            s_logger.Information(
                "{Prefix} Tournament {TournamentId} has a game mode that differs from game, can't verify game {GameId} [Tournament: Mode={TMode} | Game: Mode={GMode}",
                LogPrefix,
                tournament.Id,
                game.GameId,
                tournament.Mode,
                game.PlayMode
            );

            return false;
        }

        return true;
    }

    public static bool PassesScoringTypeCheck(Game game)
    {
        if (game.ScoringType != (int)OsuEnums.ScoringType.ScoreV2)
        {
            s_logger.Information(
                "{Prefix} Match {MatchId} does not have a ScoreV2 scoring type, can't verify game {GameId}",
                LogPrefix,
                game.Match.MatchId,
                game.GameId
            );
            return false;
        }

        return true;
    }

    public static bool PassesModsCheck(Game game) =>
        !AutomationConstants.UnallowedMods.Any(unallowedMod => game.ModsEnum.HasFlag(unallowedMod));

    public static bool PassesTeamTypeCheck(Game game)
    {
        OsuEnums.TeamType teamType = game.TeamTypeEnum;
        if (teamType is OsuEnums.TeamType.TagTeamVs or OsuEnums.TeamType.TagCoop)
        {
            s_logger.Information(
                "{Prefix} Match {MatchId} has a tag team type, can't verify game {GameId}",
                LogPrefix,
                game.Match.MatchId,
                game.GameId
            );
            return false;
        }

        // Ensure team size is valid
        if (teamType == OsuEnums.TeamType.HeadToHead)
        {
            if (game.Match.Tournament.TeamSize != 1)
            {
                s_logger.Information(
                    "{Prefix} Match {MatchId} has a HeadToHead team type, but team size is not 1, can't verify game {GameId}",
                    LogPrefix,
                    game.Match.MatchId,
                    game.GameId
                );

                return false;
            }

            return true;
        }

        // TeamVs can be used for any team size
        return true;
    }

    public static bool PassesScoreSanityCheck(Game game)
    {
        if (game.MatchScores.Count == 0)
        {
            s_logger.Warning("Game {GameId} has no scores, can't verify", game.GameId);
            return false;
        }

        return true;
    }
}
