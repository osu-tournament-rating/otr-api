using Database.Entities;
using Database.Entities.Processor;
using Database.Enums;
using Database.Enums.Verification;
using Microsoft.EntityFrameworkCore;

namespace Database.Queries.Extensions;

public static class QueryExtensions
{
    /// <summary>
    /// Gets the desired "page" of a query
    /// </summary>
    /// <param name="limit">Page size</param>
    /// <param name="page">Desired page</param>
    public static IQueryable<T> Page<T>(this IQueryable<T> query, int limit, int page) =>
        query.AsQueryable().Skip(limit * page).Take(limit);

    #region Ratings

    /// <summary>
    /// Filters a <see cref="PlayerRating"/> query for those generated for a given <see cref="Ruleset"/>
    /// </summary>
    /// <param name="ruleset">Ruleset</param>
    public static IQueryable<PlayerRating> WhereRuleset(this IQueryable<PlayerRating> query, Ruleset ruleset) =>
        query.AsQueryable().Where(x => x.Ruleset == ruleset);

    /// <summary>
    /// Orders a <see cref="PlayerRating"/> query by <see cref="PlayerRating.Rating"/> in descending order
    /// </summary>
    public static IQueryable<PlayerRating> OrderByRatingDescending(this IQueryable<PlayerRating> query) =>
        query.AsQueryable().OrderByDescending(x => x.Rating);

    #endregion

    #region Players

    /// <summary>
    /// Filters a <see cref="Player"/> query by the given osu! id
    /// </summary>
    /// <param name="osuId">osu! id</param>
    public static IQueryable<Player> WhereOsuId(this IQueryable<Player> query, long osuId) =>
        query.AsQueryable().Where(x => x.OsuId == osuId);

    /// <summary>
    /// Filters a <see cref="Player"/> query by the given the given username
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="partialMatch">
    /// If the filter should partially match the username. If false, strict matching is used
    /// </param>
    public static IQueryable<Player> WhereUsername(this IQueryable<Player> query, string username, bool partialMatch)
    {
        //_ is a wildcard character in psql so it needs to have an escape character added in front of it.
        username = username.Replace("_", @"\_");
        var pattern = partialMatch
            ? $"%{username}%"
            : username;

        return query.AsQueryable().Where(p => EF.Functions.ILike(p.Username, pattern, @"\"));
    }

    #endregion

    #region Games

    /// <summary>
    /// Filters a <see cref="Game"/> query for those with a <see cref="VerificationStatus"/>
    /// of <see cref="VerificationStatus.Verified"/>
    /// </summary>
    public static IQueryable<Game> WhereVerified(this IQueryable<Game> query) =>
        query.AsQueryable().Where(x => x.VerificationStatus == VerificationStatus.Verified);

    /// <summary>
    /// Filters a <see cref="Game"/> query for those with a <see cref="GameProcessingStatus"/>
    /// of <see cref="GameProcessingStatus.Done"/>
    /// </summary>
    public static IQueryable<Game> WhereProcessingCompleted(this IQueryable<Game> query) =>
        query.AsQueryable().Where(x => x.ProcessingStatus == GameProcessingStatus.Done);

    /// <summary>
    /// Filters a <see cref="Game"/> query based on the given <see cref="QueryFilterType"/>
    /// </summary>
    public static IQueryable<Game> WhereFiltered(this IQueryable<Game> query, QueryFilterType filterType) =>
        filterType switch
        {
            QueryFilterType.Verified => query.AsQueryable().WhereVerified(),
            QueryFilterType.ProcessingCompleted => query.AsQueryable().WhereProcessingCompleted(),
            QueryFilterType.Verified | QueryFilterType.ProcessingCompleted => query.AsQueryable().WhereVerified().WhereProcessingCompleted(),
            _ => query
        };

    /// <summary>
    /// Filters a <see cref="Game"/> query for those with a <see cref="Game.StartTime"/> that is greater than
    /// the given date
    /// </summary>
    /// <param name="date">Date comparison</param>
    public static IQueryable<Game> AfterDate(this IQueryable<Game> query, DateTime date) =>
        query.AsQueryable().Where(x => x.StartTime > date);

    /// <summary>
    /// Filters a <see cref="Game"/> query for those with a <see cref="Match.StartTime"/> that is less than
    /// the given date
    /// </summary>
    /// <param name="date">Date comparison</param>
    public static IQueryable<Game> BeforeDate(this IQueryable<Game> query, DateTime date) =>
        query.AsQueryable().Where(x => x.StartTime < date);

    /// <summary>
    /// Filters a <see cref="Game"/> query for those played between a given date range
    /// </summary>
    /// <param name="dateMin">Date range lower bound</param>
    /// <param name="dateMax">Date range upper bound</param>
    public static IQueryable<Game> WhereDateRange(
        this IQueryable<Game> query,
        DateTime dateMin,
        DateTime dateMax
    ) => query.AsQueryable().AfterDate(dateMin).BeforeDate(dateMax);

    #endregion

    #region Scores

    /// <summary>
    /// Filters a <see cref="GameScore"/> query for those with a <see cref="VerificationStatus"/>
    /// of <see cref="VerificationStatus.Verified"/>
    /// </summary>
    public static IQueryable<GameScore> WhereVerified(this IQueryable<GameScore> query) =>
        query.AsQueryable().Where(x => x.VerificationStatus == VerificationStatus.Verified);

    /// <summary>
    /// Filters a <see cref="GameScore"/> query for those with a <see cref="ScoreProcessingStatus"/>
    /// of <see cref="ScoreProcessingStatus.Done"/>
    /// </summary>
    public static IQueryable<GameScore> WhereProcessingCompleted(this IQueryable<GameScore> query) =>
        query.AsQueryable().Where(x => x.ProcessingStatus == ScoreProcessingStatus.Done);

    /// <summary>
    /// Filters a <see cref="GameScore"/> query based on the given <see cref="QueryFilterType"/>
    /// </summary>
    public static IQueryable<GameScore> WhereFiltered(this IQueryable<GameScore> query, QueryFilterType filterType) =>
        filterType switch
        {
            QueryFilterType.Verified => query.AsQueryable().WhereVerified(),
            QueryFilterType.ProcessingCompleted => query.AsQueryable().WhereProcessingCompleted(),
            QueryFilterType.Verified | QueryFilterType.ProcessingCompleted => query.AsQueryable().WhereVerified().WhereProcessingCompleted(),
            _ => query
        };

    /// <summary>
    /// Filters a <see cref="GameScore"/> query for those set after a given date
    /// </summary>
    /// <param name="date">Date comparison</param>
    public static IQueryable<GameScore> AfterDate(this IQueryable<GameScore> query, DateTime date) =>
        query.AsQueryable().Where(x => x.Game.EndTime > date);

    /// <summary>
    /// Filters a <see cref="GameScore"/> query for those set before a given date
    /// </summary>
    /// <param name="date">Date comparison</param>
    public static IQueryable<GameScore> BeforeDate(this IQueryable<GameScore> query, DateTime date) =>
        query.AsQueryable().Where(x => x.Game.EndTime < date);

    /// <summary>
    /// Filters a <see cref="GameScore"/> query for those set between a given date range
    /// </summary>
    /// <param name="dateMin">Date range lower bound</param>
    /// <param name="dateMax">Date range upper bound</param>
    public static IQueryable<GameScore> WhereDateRange(
        this IQueryable<GameScore> query,
        DateTime dateMin,
        DateTime dateMax
    ) => query.AsQueryable().AfterDate(dateMin).BeforeDate(dateMax);

    /// <summary>
    /// Filters a <see cref="GameScore"/> query for those set in a given <see cref="Ruleset"/>
    /// </summary>
    /// <param name="ruleset">Ruleset</param>
    public static IQueryable<GameScore> WhereRuleset(this IQueryable<GameScore> query, Ruleset ruleset) =>
        query.AsQueryable().Where(x => x.Ruleset == ruleset);

    /// <summary>
    /// Filters a <see cref="GameScore"/> query for those set with the given <see cref="Mods"/> enabled
    /// </summary>
    /// <param name="enabledMods">Mods</param>
    public static IQueryable<GameScore> WhereMods(
        this IQueryable<GameScore> query,
        Mods enabledMods
    ) =>
        query
            .AsQueryable()
            .Where(x =>
                x.Game.Mods == enabledMods
                || x.Game.Mods == (enabledMods | Mods.NoFail)
                || x.Mods == enabledMods
                || x.Mods == (enabledMods | Mods.NoFail)
            );

    /// <summary>
    /// Filters a <see cref="GameScore"/> query for those where the <see cref="Player"/> who set the score is
    /// not on the same team as the <see cref="Player"/> with the given osu! id
    /// </summary>
    /// <param name="osuId">osu! id</param>
    public static IQueryable<GameScore> WhereOpponentOf(this IQueryable<GameScore> query, long osuId) =>
        query
            .AsQueryable()
            .Where(x =>
                x.Game.Scores.Any(y => y.Player.OsuId == osuId)
                && x.Player.OsuId != osuId
                && x.Team != x.Game.Scores.First(y => y.Player.OsuId == osuId).Team
            );

    /// <summary>
    /// Filters a <see cref="GameScore"/> query for those where the <see cref="Player"/> who set the score is
    /// on the same team as the <see cref="Player"/> with the given osu! id
    /// </summary>
    /// <param name="osuId">osu! id</param>
    public static IQueryable<GameScore> WhereTeammateOf(this IQueryable<GameScore> query, long osuId) =>
        query
            .AsQueryable()
            .Where(x =>
                x.Game.Scores.Any(y => y.Player.OsuId == osuId)
                && x.Player.OsuId != osuId
                && x.Team == x.Game.Scores.First(y => y.Player.OsuId == osuId).Team
            );

    public static IQueryable<GameScore> WherePlayerId(this IQueryable<GameScore> query, int playerId) =>
        query.AsQueryable().Where(x => x.PlayerId == playerId);

    #endregion
}
