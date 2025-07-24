using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;
using DWS.Utilities;
using DWS.Utilities.Extensions;

namespace DWS.AutomationChecks;

public class GameAutomationChecks(ILogger<GameAutomationChecks> logger)
{
    private static readonly IEnumerable<Mods> _invalidMods = [Mods.SuddenDeath, Mods.Perfect, Mods.Relax, Mods.Autoplay, Mods.Relax2];
    public GameRejectionReason Process(Game game, Tournament tournament)
    {
        logger.LogTrace("Processing game {GameId}", game.Id);

        GameRejectionReason result = GameBeatmapUsageCheck(game, tournament) |
                                     GameEndTimeCheck(game) |
                                     GameModCheck(game) |
                                     GameRulesetCheck(game, tournament) |
                                     GameScoreCountCheck(game, tournament) |
                                     GameScoringTypeCheck(game) |
                                     GameTeamTypeCheck(game);

        logger.LogTrace("Game {GameId} processed with rejection reason: {RejectionReason}", game.Id, result);
        return result;
    }

    private GameRejectionReason GameTeamTypeCheck(Game game)
    {
        if (game.TeamType == TeamType.TeamVs)
        {
            return GameRejectionReason.None;
        }

        logger.LogTrace("Game {GameId} failed team type check. Team type: {TeamType}", game.Id, game.TeamType);
        return GameRejectionReason.InvalidTeamType;
    }

    private GameRejectionReason GameScoringTypeCheck(Game game)
    {
        if (game.ScoringType == ScoringType.ScoreV2)
        {
            return GameRejectionReason.None;
        }

        logger.LogTrace("Game {GameId} failed scoring type check. Scoring type: {ScoringType}", game.Id, game.ScoringType);
        return GameRejectionReason.InvalidScoringType;
    }

    private GameRejectionReason GameScoreCountCheck(Game game, Tournament tournament)
    {
        // Game has no scores at all
        if (game.Scores.Count == 0)
        {
            logger.LogTrace("Game {GameId} failed score count check. No scores present", game.Id);
            return GameRejectionReason.NoScores;
        }

        GameScore[] validScores = [.. game.Scores.Where(gs => gs.VerificationStatus.IsPreVerifiedOrVerified())];
        int validScoresCount = validScores.Length;

        // Game has no valid scores
        if (validScoresCount == 0)
        {
            logger.LogTrace("Game {GameId} failed score count check. No valid scores present", game.Id);
            return GameRejectionReason.NoValidScores;
        }

        if (validScoresCount % 2 == 0 && validScoresCount / 2 == tournament.LobbySize)
        {
            ICollection<GameRoster> rosters = RostersHelper.GenerateRosters(validScores);

            if (rosters.Count > 1 &&
                rosters.DistinctBy(x => x.Roster.Length).Count() == 1)
            {
                return GameRejectionReason.None;
            }
        }

        logger.LogTrace("Game {GameId} failed score count check. Lobby size mismatch", game.Id);
        return GameRejectionReason.LobbySizeMismatch;
    }

    private GameRejectionReason GameRulesetCheck(Game game, Tournament tournament)
    {
        if (game.Ruleset == tournament.Ruleset)
        {
            return GameRejectionReason.None;
        }

        logger.LogTrace("Game {GameId} failed ruleset check. Game Ruleset: {GameRuleset}, Tournament Ruleset: {TournamentRuleset}",
                        game.Id, game.Ruleset, tournament.Ruleset);
        return GameRejectionReason.RulesetMismatch;
    }

    private GameRejectionReason GameModCheck(Game game)
    {
        if (_invalidMods.All(m => !game.Mods.HasFlag(m)))
        {
            return GameRejectionReason.None;
        }

        logger.LogTrace("Game {GameId} failed mod check. Invalid mods present: {Mods}", game.Id, game.Mods);
        return GameRejectionReason.InvalidMods;
    }

    private GameRejectionReason GameEndTimeCheck(Game game)
    {
        if (!game.EndTime.IsPlaceholder())
        {
            return GameRejectionReason.None;
        }

        logger.LogTrace("Game {GameId} failed end time check. End time: {EndTime}", game.Id, game.EndTime);
        return GameRejectionReason.NoEndTime;
    }

    private static GameRejectionReason GameBeatmapUsageCheck(Game game, Tournament tournament)
    {
        if (game.Beatmap is null)
        {
            return GameRejectionReason.None;
        }

        // If the tournament has a mappool
        if (tournament.PooledBeatmaps.Count == 0)
        {
            /**
             * Scan all games in the entire tournament.
             * If there is exactly 1 game which uses this beatmap,
             * flag it.
             */
            if (tournament.Matches
                    .SelectMany(m => m.Games)
                    .Select(g => g.Beatmap?.OsuId)
                    .Count(id => id == game.Beatmap.OsuId) == 1)
            {
                game.WarningFlags |= GameWarningFlags.BeatmapUsedOnce;
            }

            return GameRejectionReason.None;
        }

        // If the game's map is in the pool, don't mark with any flags
        if (tournament.PooledBeatmaps.Select(b => b.OsuId).Contains(game.Beatmap.OsuId))
        {
            return GameRejectionReason.None;
        }

        // The tournament has a mappool, but this beatmap is not present in it.
        // Mark as rejected.
        return GameRejectionReason.BeatmapNotPooled;
    }
}
