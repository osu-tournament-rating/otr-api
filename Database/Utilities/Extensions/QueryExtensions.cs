using Database.Entities;
using Database.Entities.Processor;
using Database.Enums;
using Database.Enums.Verification;
using Microsoft.EntityFrameworkCore;

namespace Database.Utilities.Extensions;

public static class QueryExtensions
{
    // Player
    public static IQueryable<Player> WhereOsuId(this IQueryable<Player> query, long osuId) =>
        query.AsQueryable().Where(x => x.OsuId == osuId);

    /// <summary>
    /// Filters a query for players with the given username
    /// </summary>
    public static IQueryable<Player> WhereUsername(this IQueryable<Player> query, string username, bool partialMatch)
    {
        //_ is a wildcard character in psql so it needs to have an escape character added in front of it.
        username = username.Replace("_", @"\_");
        var pattern = partialMatch
            ? $"%{username}%"
            : username;

        return query
            .AsQueryable()
            .Where(p =>
                p.Username != null
                && EF.Functions.ILike(p.Username ?? string.Empty, pattern, @"\")
            );
    }

    // Match
    public static IQueryable<Match> WhereVerified(this IQueryable<Match> query) =>
        query
            .AsQueryable()
            .Where(x => x.VerificationStatus == VerificationStatus.Verified);

    public static IQueryable<Match> After(this IQueryable<Match> query, DateTime after) =>
        query.AsQueryable().Where(x => x.StartTime > after);

    public static IQueryable<Match> Before(this IQueryable<Match> query, DateTime before) =>
        query.AsQueryable().Where(x => x.StartTime < before);

    public static IQueryable<Match> WhereRuleset(this IQueryable<Match> query, Ruleset ruleset) =>
        query.AsQueryable().Where(x => x.Tournament.Ruleset == ruleset);

    public static IQueryable<Match> IncludeAllChildren(this IQueryable<Match> query) =>
        query
            .AsQueryable()
            .Include(x => x.Games)
            .ThenInclude(x => x.Scores)
            .Include(x => x.Games)
            .ThenInclude(x => x.Beatmap);

    public static IQueryable<Match> WherePlayerParticipated(this IQueryable<Match> query, long osuPlayerId) =>
        query
            .AsQueryable()
            .Where(x => x.Games.Any(y => y.Scores.Any(z => z.Player.OsuId == osuPlayerId)));

    // Game
    public static IQueryable<Game> WhereVerified(this IQueryable<Game> query) =>
        query
            .AsQueryable()
            .Where(x => x.VerificationStatus == VerificationStatus.Verified);

    public static IQueryable<Game> WhereTeamVs(this IQueryable<Game> query) =>
        query.AsQueryable().Where(x => x.TeamType == TeamType.TeamVs);

    public static IQueryable<Game> WhereHeadToHead(this IQueryable<Game> query) =>
        query.AsQueryable().Where(x => x.TeamType == TeamType.HeadToHead);

    public static IQueryable<Game> After(this IQueryable<Game> query, DateTime after) =>
        query.AsQueryable().Where(x => x.StartTime > after);

    /// <summary>
    ///  Returns all MatchScores where either the game or the player had the specified mods enabled
    /// </summary>
    /// <param name="query"></param>
    /// <param name="enabledMods"></param>
    /// <returns></returns>
    public static IQueryable<GameScore> WhereMods(
        this IQueryable<GameScore> query,
        Mods enabledMods
    )
    {
        return query
            .AsQueryable()
            .Where(x =>
                (x.Game.Mods != Mods.None && x.Game.Mods == enabledMods)
                || // Not using NF
                (x.Mods == enabledMods)
                || (x.Game.Mods != Mods.None && x.Game.Mods == (enabledMods | Mods.NoFail))
                || // Using NF
                x.Mods == (enabledMods | Mods.NoFail)
            );
    }

    // MatchScore
    /// <summary>
    ///  Selects scores that are verified
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public static IQueryable<GameScore> WhereVerified(this IQueryable<GameScore> query) =>
        query
            .AsQueryable()
            .Where(x =>
                x.VerificationStatus == VerificationStatus.Verified
                && x.Game.Match.VerificationStatus == VerificationStatus.Verified
                && x.Game.VerificationStatus == VerificationStatus.Verified
            );

    /// <summary>
    ///  Selects all HeadToHead scores
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public static IQueryable<GameScore> WhereHeadToHead(this IQueryable<GameScore> query) =>
        query.AsQueryable().Where(x => x.Game.TeamType == TeamType.HeadToHead);

    public static IQueryable<GameScore> WhereNotHeadToHead(this IQueryable<GameScore> query) =>
        query.AsQueryable().Where(x => x.Game.TeamType != TeamType.HeadToHead);

    /// <summary>
    ///  Selects all TeamVs match scores
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public static IQueryable<GameScore> WhereTeamVs(this IQueryable<GameScore> query) =>
        query.AsQueryable().Where(x => x.Game.TeamType == TeamType.TeamVs);

    /// <summary>
    ///  Selects all match scores, other than the provided player's, that are on the opposite team as the provided player.
    ///  Excludes HeadToHead scores
    /// </summary>
    /// <param name="query"></param>
    /// <param name="osuPlayerId"></param>
    /// <returns></returns>
    public static IQueryable<GameScore> WhereOpponent(this IQueryable<GameScore> query, long osuPlayerId) =>
        query
            .AsQueryable()
            .Where(x =>
                x.Game.Scores.Any(y => y.Player.OsuId == osuPlayerId)
                && x.Player.OsuId != osuPlayerId
                && x.Team != x.Game.Scores.First(y => y.Player.OsuId == osuPlayerId).Team
            );

    /// <summary>
    ///  Selects all match scores, other than the provided player's, that are on the same team as the provided player. Excludes
    ///  HeadToHead scores
    /// </summary>
    /// <param name="query"></param>
    /// <param name="osuPlayerId"></param>
    /// <returns></returns>
    public static IQueryable<GameScore> WhereTeammate(this IQueryable<GameScore> query, long osuPlayerId) =>
        query
            .AsQueryable()
            .Where(x =>
                x.Game.Scores.Any(y => y.Player.OsuId == osuPlayerId)
                && x.Player.OsuId != osuPlayerId
                && x.Game.TeamType != TeamType.HeadToHead
                && x.Team == x.Game.Scores.First(y => y.Player.OsuId == osuPlayerId).Team
            );

    public static IQueryable<GameScore> WhereDateRange(
        this IQueryable<GameScore> query,
        DateTime dateMin,
        DateTime dateMax
    ) => query.AsQueryable().Where(x => x.Game.StartTime > dateMin && x.Game.StartTime < dateMax);

    /// <summary>
    /// Selects all MatchScores for a given ruleset (e.g. mania)
    /// </summary>
    public static IQueryable<GameScore> WhereRuleset(this IQueryable<GameScore> query, Ruleset ruleset) =>
        query.AsQueryable().Where(x => x.Game.Ruleset == ruleset);

    public static IQueryable<GameScore> WhereOsuPlayerId(
        this IQueryable<GameScore> query,
        long osuPlayerId
    ) => query.AsQueryable().Where(x => x.Player.OsuId == osuPlayerId);

    public static IQueryable<GameScore> WherePlayerId(this IQueryable<GameScore> query, int playerId) =>
        query.AsQueryable().Where(x => x.PlayerId == playerId);

    public static IQueryable<GameScore> After(this IQueryable<GameScore> query, DateTime after) =>
        query.AsQueryable().Where(x => x.Game.StartTime > after);

    // Rating
    public static IQueryable<PlayerRating> WhereRuleset(this IQueryable<PlayerRating> query, Ruleset ruleset) =>
        query.AsQueryable().Where(x => x.Ruleset == ruleset);

    public static IQueryable<PlayerRating> WhereOsuPlayerId(
        this IQueryable<PlayerRating> query,
        long osuPlayerId
    ) => query.AsQueryable().Where(x => x.Player.OsuId == osuPlayerId);

    public static IQueryable<PlayerRating> OrderByRatingDescending(this IQueryable<PlayerRating> query) =>
        query.AsQueryable().OrderByDescending(x => x.Rating);
}
