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
        var validScores = game.MatchScores.Where(x => x.IsValid == true).ToList();

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
            var satisfiesOneVersusOne = validScores.Count == 2;
            if (!satisfiesOneVersusOne)
            {
                s_logger.Information(
                    "{Prefix} Match {MatchId} has a team size of 1, but does not satisfy the 1v1 checks, can't verify game {GameId}",
                    LogPrefix,
                    game.Match.MatchId,
                    game.GameId
                );
            }

            return satisfiesOneVersusOne;
        }

        var countRed = validScores.Count(s => s.Team == (int)Team.Red);
        var countBlue = validScores.Count(s => s.Team == (int)Team.Blue);

        if (countRed == 0 || countBlue == 0)
        {
            // Situation where either no scores or valid, or there are only scores from one team, something isn't right.
            s_logger.Information(
                "{Prefix} Match {MatchId} has no team size for red or blue, can't verify game {GameId} (likely a warmup) [Red: {Red} | Blue: {Blue}]",
                LogPrefix,
                game.Match.MatchId,
                game.GameId,
                countRed,
                countBlue
            );

            return false;
        }

        if (countRed != countBlue)
        {
            // We don't need to worry about referees here as they have already been marked as invalid in the scores list.
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

        if (countRed != teamSize || countBlue != teamSize)
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

        return true;
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
