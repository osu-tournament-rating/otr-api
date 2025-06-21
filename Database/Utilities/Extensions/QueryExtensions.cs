using Common.Enums;
using Common.Enums.Queries;
using Common.Enums.Verification;
using Database.Entities;
using Database.Entities.Interfaces;
using Database.Entities.Processor;
using Microsoft.EntityFrameworkCore;

namespace Database.Utilities.Extensions;

public static class QueryExtensions
{
    #region Beatmaps

    public static IQueryable<Beatmap> IncludeChildren(this IQueryable<Beatmap> query) =>
        query
            .AsSplitQuery()
            .Include(b => b.Beatmapset)
            .Include(b => b.Creators)
            .Include(b => b.Attributes);

    #endregion

    #region Ratings

    /// <summary>
    /// Filters a <see cref="PlayerRating"/> query for those generated for a given <see cref="Ruleset"/>
    /// </summary>
    /// <param name="ruleset">Ruleset</param>
    public static IQueryable<PlayerRating> WhereRuleset(this IQueryable<PlayerRating> query, Ruleset ruleset) =>
        query.AsQueryable().Where(x => x.Ruleset == ruleset);
    #endregion

    #region Players

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
        string pattern = partialMatch
            ? $"%{username}%"
            : username;

        return query.AsQueryable().Where(p => EF.Functions.ILike(p.Username, pattern, @"\"));
    }

    #endregion

    #region Tournaments

    /// <summary>
    /// Filters a <see cref="Tournament"/> query for those with the given
    /// <see cref="VerificationStatus"/>
    /// </summary>
    /// <param name="verificationStatus">Verification status</param>
    /// <remarks>Does nothing if <paramref name="verificationStatus"/> is null</remarks>
    public static IQueryable<Tournament> WhereVerificationStatus(
        this IQueryable<Tournament> query,
        VerificationStatus? verificationStatus = null
    ) =>
        verificationStatus.HasValue
            ? query.Where(e => e.VerificationStatus == verificationStatus.Value)
            : query;

    /// <summary>
    /// Filters a <see cref="Tournament"/> query for those with the given
    /// <see cref="TournamentProcessingStatus"/>
    /// </summary>
    /// <param name="processingStatus">Processing status</param>
    /// <remarks>Does nothing if <paramref name="processingStatus"/> is null</remarks>
    public static IQueryable<Tournament> WhereProcessingStatus(
        this IQueryable<Tournament> query,
        TournamentProcessingStatus? processingStatus = null
    ) =>
        processingStatus.HasValue
            ? query.Where(t => t.ProcessingStatus == processingStatus.Value)
            : query;

    /// <summary>
    /// Filters a <see cref="Tournament"/> query for those with the given
    /// <see cref="TournamentRejectionReason"/>
    /// </summary>
    /// <param name="rejectionReason">Rejection reason</param>
    /// <remarks>Does nothing if <paramref name="rejectionReason"/> is null</remarks>
    public static IQueryable<Tournament> WhereRejectionReason(
        this IQueryable<Tournament> query,
        TournamentRejectionReason? rejectionReason = null
    ) =>
        rejectionReason.HasValue
            ? query.Where(t => t.RejectionReason == rejectionReason.Value)
            : query;

    /// <summary>
    /// Filters a <see cref="Tournament"/> query for those played in the given <see cref="Ruleset"/>
    /// </summary>
    /// <param name="ruleset">Ruleset</param>
    /// <remarks>Does nothing if <paramref name="ruleset"/> is null</remarks>
    public static IQueryable<Tournament> WhereRuleset(this IQueryable<Tournament> query, Ruleset? ruleset) =>
        ruleset.HasValue ? query.Where(t => t.Ruleset == ruleset.Value) : query;

    /// <summary>
    /// Filters a <see cref="Tournament"/> query for those with a name or abbreviation that partially matches the
    /// given search term
    /// </summary>
    /// <param name="searchQuery">String to match against name and abbreviation</param>
    /// <remarks>Does nothing if <paramref name="searchQuery"/> is null</remarks>
    public static IQueryable<Tournament> WhereSearchQuery(this IQueryable<Tournament> query, string? searchQuery = null)
    {
        if (string.IsNullOrEmpty(searchQuery))
        {
            return query;
        }

        //_ is a wildcard character in psql so it needs to have an escape character added in front of it.
        searchQuery = searchQuery.Replace("_", @"\_");

        return query.Where(t =>
            EF.Functions.ILike(t.Name, $"%{searchQuery}%", @"\")
            || EF.Functions.ILike(t.Abbreviation, $"%{searchQuery}%", @"\")
        );
    }

    /// <summary>
    /// Filters a <see cref="Tournament"/> query for those with a <see cref="Tournament.EndTime"/> that is on or
    /// after the given date
    /// </summary>
    /// <param name="date">Date comparison</param>
    /// <remarks>Does nothing if <paramref name="date"/> is null</remarks>
    private static IQueryable<Tournament> AfterDate(this IQueryable<Tournament> query, DateTime? date = null) =>
        date.HasValue ? query.Where(t => t.EndTime >= date.Value) : query;

    /// <summary>
    /// Filters a <see cref="Tournament"/> query for those with a <see cref="Tournament.StartTime"/> that is on or
    /// before the given date
    /// </summary>
    /// <param name="date">Date comparison</param>
    /// <remarks>Does nothing if <paramref name="date"/> is null</remarks>
    private static IQueryable<Tournament> BeforeDate(this IQueryable<Tournament> query, DateTime? date = null) =>
        date.HasValue ? query.Where(t => t.StartTime <= date) : query;

    /// <summary>
    /// Filters a <see cref="Tournament"/> query for those played between a given date range
    /// </summary>
    /// <param name="dateMin">Date range lower bound</param>
    /// <param name="dateMax">Date range upper bound</param>
    /// <remarks>
    /// If either <param name="dateMin"> or <param name="dateMax"> are null, only filters for the end of the
    /// range that is included. Does nothing if both are null.
    /// </remarks>
    public static IQueryable<Tournament> WhereDateRange(
        this IQueryable<Tournament> query,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    ) => query.AsQueryable().AfterDate(dateMin).BeforeDate(dateMax);

    /// <summary>
    /// Filters a <see cref="Tournament"/> query for those submitted by a user matching the given id
    /// </summary>
    /// <param name="userId">User id</param>
    /// <remarks>Does nothing if <paramref name="userId"/> is null</remarks>
    public static IQueryable<Tournament> WhereSubmittedBy(this IQueryable<Tournament> query, int? userId = null) =>
        userId.HasValue ? query.Where(t => t.SubmittedByUserId == userId.Value) : query;

    /// <summary>
    /// Filters a <see cref="Tournament"/> query for those verified by a user matching the given id
    /// </summary>
    /// <param name="userId">User id</param>
    /// <remarks>Does nothing if <paramref name="userId"/> is null</remarks>
    public static IQueryable<Tournament> WhereVerifiedBy(this IQueryable<Tournament> query, int? userId = null) =>
        userId.HasValue ? query.Where(t => t.VerifiedByUserId == userId.Value) : query;

    /// <summary>
    /// Filters a <see cref="Tournament"/> query for those played with the given lobby size
    /// </summary>
    /// <param name="lobbySize">Lobby size</param>
    /// <remarks>Does nothing if <paramref name="lobbySize"/> is null</remarks>
    public static IQueryable<Tournament> WhereLobbySize(this IQueryable<Tournament> query, int? lobbySize = null) =>
        lobbySize.HasValue ? query.Where(t => t.LobbySize == lobbySize.Value) : query;

    /// <summary>
    /// Orders the query based on the specified sort type and direction.
    /// </summary>
    /// <param name="query">The query to be ordered.</param>
    /// <param name="sortType">Defines which key to order the results by</param>
    /// <param name="descending">A boolean indicating whether the ordering should be in descending order. Defaults to false (ascending).</param>
    /// <returns>The ordered query</returns>
    public static IQueryable<Tournament> OrderBy(this IQueryable<Tournament> query, TournamentQuerySortType sortType,
        bool descending = true) =>
        sortType switch
        {
            TournamentQuerySortType.Id => descending ? query.OrderByDescending(t => t.Id) : query.OrderBy(t => t.Id),
            TournamentQuerySortType.SearchQueryRelevance => descending
                ? query.OrderByDescending(t => t.Name)
                : query.OrderBy(t => t.Name),
            TournamentQuerySortType.StartTime => descending
                ? query.OrderByDescending(t => t.StartTime ?? DateTime.MinValue)
                : query.OrderBy(t => t.StartTime),
            TournamentQuerySortType.EndTime => descending
                ? query.OrderByDescending(t => t.EndTime ?? DateTime.MinValue)
                : query.OrderBy(t => t.EndTime),
            TournamentQuerySortType.SubmissionDate => descending
                ? query.OrderByDescending(t => t.Created)
                : query.OrderBy(t => t.Created),
            TournamentQuerySortType.LobbySize => descending
                ? query.OrderByDescending(t => t.LobbySize)
                : query.OrderBy(t => t.LobbySize),
            _ => query
        };

    #endregion

    #region Matches

    /// <summary>
    /// Filters a <see cref="Match"/> query for those with a <see cref="VerificationStatus"/>
    /// of <see cref="VerificationStatus.Verified"/>
    /// </summary>
    public static IQueryable<Match> WhereVerified(this IQueryable<Match> query) =>
        query.AsQueryable().Where(x => x.VerificationStatus == VerificationStatus.Verified);

    /// <summary>
    /// Filters a <see cref="Match"/> query for those with a <see cref="MatchProcessingStatus"/>
    /// of <see cref="MatchProcessingStatus.Done"/>
    /// </summary>
    public static IQueryable<Match> WhereProcessingCompleted(this IQueryable<Match> query) =>
        query.AsQueryable().Where(x => x.ProcessingStatus == MatchProcessingStatus.Done);

    /// <summary>
    /// Includes navigation properties for a <see cref="Match"/>
    /// <br/>Includes: <see cref="Match.Rosters"/>, <see cref="Match.PlayerMatchStats"/>,
    /// <see cref="Match.PlayerRatingAdjustments"/>, <see cref="Match.Games"/>
    /// (<see cref="Game.Scores"/>, <see cref="Game.Beatmap"/>, <see cref="Game.Rosters"/>)
    /// </summary>
    public static IQueryable<Match> IncludeChildren(this IQueryable<Match> query)
    {
        return query
            .AsSplitQuery()
            .Include(m => m.Rosters)
            .Include(m => m.PlayerMatchStats)
            .Include(m => m.PlayerRatingAdjustments)
            .ThenInclude(ra => ra.Player)
            .Include(m => m.Games)
            .ThenInclude(g => g.Scores)
            .ThenInclude(gs => gs.AdminNotes)
            .ThenInclude(an => an.AdminUser.Player)
            .Include(m => m.Games)
            .ThenInclude(g => g.AdminNotes)
            .ThenInclude(an => an.AdminUser.Player)
            .Include(m => m.Games)
            .ThenInclude(g => g.Beatmap)
            .ThenInclude(b => b!.Beatmapset)
            .ThenInclude(bs => bs!.Creator)
            .Include(m => m.Games)
            .ThenInclude(g => g.Beatmap)
            .ThenInclude(b => b!.Creators)
            .Include(m => m.Games)
            .ThenInclude(g => g.Rosters);
    }

    /// <summary>
    /// Includes the <see cref="Tournament"/> navigation on this <see cref="Match"/>
    /// </summary>
    /// <param name="query">The <see cref="Match"/> query</param>
    /// <returns>The query with the <see cref="Tournament"/> included</returns>
    public static IQueryable<Match> IncludeTournament(this IQueryable<Match> query) =>
        query
            .AsQueryable()
            .Include(m => m.Tournament);

    /// <summary>
    /// Filters a <see cref="Match"/> query for those with a <see cref="Match.StartTime"/> that is greater than
    /// the given date
    /// </summary>
    /// <param name="date">Date comparison</param>
    public static IQueryable<Match> AfterDate(this IQueryable<Match> query, DateTime date) =>
        query.AsQueryable().Where(x => x.StartTime > date);

    /// <summary>
    /// Filters a <see cref="Match"/> query for those with a <see cref="Match.StartTime"/> that is less than
    /// the given date
    /// </summary>
    /// <param name="date">Date comparison</param>
    public static IQueryable<Match> BeforeDate(this IQueryable<Match> query, DateTime date) =>
        query.AsQueryable().Where(x => x.StartTime < date);

    /// <summary>
    /// Filters a <see cref="Match"/> query for those played between a given date range
    /// </summary>
    /// <param name="dateMin">Date range lower bound</param>
    /// <param name="dateMax">Date range upper bound</param>
    public static IQueryable<Match> WhereDateRange(
        this IQueryable<Match> query,
        DateTime dateMin,
        DateTime dateMax
    ) => query.AsQueryable().AfterDate(dateMin).BeforeDate(dateMax);

    /// <summary>
    /// Filters a <see cref="Match"/> query for those played in the given <see cref="Ruleset"/>
    /// </summary>
    /// <param name="ruleset">Ruleset</param>
    public static IQueryable<Match> WhereRuleset(this IQueryable<Match> query, Ruleset ruleset) =>
        query.AsQueryable().Where(x => x.Tournament.Ruleset == ruleset);

    /// <summary>
    /// Filters a <see cref="Match"/> query for those with a partial match of the given name
    /// </summary>
    /// <param name="name">Match name</param>
    public static IQueryable<Match> WhereName(this IQueryable<Match> query, string name)
    {
        name = name.Replace("_", @"\_");
        return query
            .AsQueryable()
            .Where(x => EF.Functions.ILike(x.Name, $"%{name}%", @"\"));
    }

    /// <summary>
    /// Orders the query based on the specified sort type and direction.
    /// </summary>
    /// <param name="query">The query to be ordered.</param>
    /// <param name="sortType">Defines which key to order the results by</param>
    /// <param name="descending">A boolean indicating whether the ordering should be in descending order. Defaults to false (ascending).</param>
    /// <returns>The ordered query</returns>
    public static IQueryable<Match> OrderBy(this IQueryable<Match> query, MatchQuerySortType sortType,
        bool descending = false) =>
        sortType switch
        {
            MatchQuerySortType.Id => descending ? query.OrderByDescending(m => m.Id) : query.OrderBy(m => m.Id),
            MatchQuerySortType.OsuId =>
                descending ? query.OrderByDescending(m => m.OsuId) : query.OrderBy(m => m.OsuId),
            MatchQuerySortType.StartTime => descending
                ? query.OrderByDescending(m => m.StartTime ?? DateTime.MinValue)
                : query.OrderBy(m => m.StartTime),
            MatchQuerySortType.EndTime => descending
                ? query.OrderByDescending(m => m.EndTime ?? DateTime.MinValue)
                : query.OrderBy(m => m.EndTime),
            MatchQuerySortType.Created => descending
                ? query.OrderByDescending(m => m.Created)
                : query.OrderBy(m => m.Created),
            _ => query
        };


    /// <summary>
    /// Filters a <see cref="Match"/> query for those where a <see cref="Player"/> with the given osu! id participated
    /// </summary>
    /// <remarks>
    /// Does not filter for <see cref="VerificationStatus"/> or <see cref="MatchProcessingStatus"/>. Should only be
    /// used after filtering for validity
    /// </remarks>
    /// <param name="osuPlayerId">osu! id of the target <see cref="Player"/></param>
    public static IQueryable<Match> WherePlayerParticipated(this IQueryable<Match> query, long osuPlayerId) =>
        query.AsQueryable().Where(x => x.Games.Any(y => y.Scores.Any(z => z.Player.OsuId == osuPlayerId)));

    #endregion

    #region Games

    /// <summary>
    /// Includes navigation properties for a <see cref="Game"/>
    /// <br/>Includes: <see cref="Game.Match"/>, <see cref="Game.Beatmap"/>, <see cref="Game.Rosters"/>,
    /// <see cref="Game.Scores"/>, <see cref="Game.AdminNotes"/>,
    /// <see cref="Game.Audits"/>
    /// </summary>
    /// <param name="verified">Whether all navigations must be verified</param>
    public static IQueryable<Game> IncludeChildren(this IQueryable<Game> query, bool verified)
    {
        query = query.AsSplitQuery();

        if (verified)
        {
            query = query.Include(g => g.Scores.Where(s => s.VerificationStatus == VerificationStatus.Verified &&
                                                           s.ProcessingStatus == ScoreProcessingStatus.Done));
        }

        return query
            .Include(g => g.Match)
            .Include(g => g.Beatmap)
            .ThenInclude(b => b!.Beatmapset)
            .ThenInclude(bs => bs!.Creator)
            .Include(g => g.Beatmap)
            .ThenInclude(b => b!.Creators)
            .Include(g => g.Rosters)
            .Include(g => g.Scores)
            .ThenInclude(s => s.Player)
            .Include(g => g.Scores)
            .ThenInclude(gs => gs.AdminNotes)
            .ThenInclude(an => an.AdminUser.Player)
            .Include(g => g.AdminNotes)
            .ThenInclude(an => an.AdminUser.Player);
    }
    #endregion

    #region Scores
    public static IQueryable<GameScore> WherePlayerId(this IQueryable<GameScore> query, int playerId) =>
        query.AsQueryable().Where(x => x.PlayerId == playerId);

    #endregion

    #region Admin Notes

    /// <summary>
    /// Includes admin notes, with all required children, for a given <typeparamref name="TEntity"/>
    /// </summary>
    /// <param name="query"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TAdminNote"></typeparam>
    /// <returns></returns>
    public static IQueryable<TEntity> IncludeAdminNotes<TEntity, TAdminNote>(this IQueryable<TEntity> query)
        where TEntity : class, IAdminNotableEntity<TAdminNote>
        where TAdminNote : IAdminNoteEntity
        => query.Include(e => e.AdminNotes)
            .ThenInclude(an => an.AdminUser.Player);

    #endregion
}
