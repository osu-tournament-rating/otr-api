using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;
using Database.Queries.Enums;
using Database.Queries.Filters;
using Microsoft.EntityFrameworkCore;

namespace Database.Queries.Extensions;

public static class MatchQueryExtensions
{
    public static IQueryable<Match> WhereFiltered(this IQueryable<Match> query, MatchesQueryFilter filter)
    {
        if (filter.Ruleset.HasValue)
        {
            query = query.WhereRuleset(filter.Ruleset.Value);
        }

        if (!string.IsNullOrEmpty(filter.Name))
        {
            query = query.WhereName(filter.Name);
        }

        if (filter.DateMin.HasValue)
        {
            query = query.AfterDate(filter.DateMin.Value);
        }

        if (filter.DateMax.HasValue)
        {
            query = query.BeforeDate(filter.DateMax.Value);
        }

        if (filter.VerificationStatus.HasValue)
        {
            query = query.Where(m => m.VerificationStatus == filter.VerificationStatus.Value);
        }

        if (filter.RejectionReason.HasValue)
        {
            query = query.Where(m => m.RejectionReason == filter.RejectionReason.Value);
        }

        if (filter.ProcessingStatus.HasValue)
        {
            query = query.Where(m => m.ProcessingStatus == filter.ProcessingStatus.Value);
        }

        if (filter.SubmittedBy.HasValue)
        {
            query = query.Where(m => m.SubmittedByUserId == filter.SubmittedBy.Value);
        }

        if (filter.VerifiedBy.HasValue)
        {
            query = query.Where(m => m.VerifiedByUserId == filter.VerifiedBy.Value);
        }

        query = query.OrderBy(filter.Sort ?? MatchesQuerySortType.Id, filter.SortDescending ?? false);

        return query;
    }

    /// <summary>
    /// Filters a <see cref="Match"/> query based on the given <see cref="QueryFilterType"/>
    /// </summary>
    public static IQueryable<Match> WhereFiltered(this IQueryable<Match> query, QueryFilterType filterType) =>
        filterType switch
        {
            QueryFilterType.Verified => query.AsQueryable().WhereVerified(),
            QueryFilterType.ProcessingCompleted => query.AsQueryable().WhereProcessingCompleted(),
            QueryFilterType.Verified | QueryFilterType.ProcessingCompleted => query.AsQueryable().WhereVerified().WhereProcessingCompleted(),
            _ => query
        };

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
    /// Includes child navigation properties for a <see cref="Match"/> query based on the given <see cref="QueryFilterType"/>
    /// <br/>Includes: <see cref="Match.Games"/> (<see cref="Game.Scores"/>, <see cref="Game.Beatmap"/>)
    /// </summary>
    /// <param name="filterType">A <see cref="QueryFilterType"/> that controls the way children are included</param>
    public static IQueryable<Match> IncludeChildren(this IQueryable<Match> query, QueryFilterType filterType) =>
        query
            .AsQueryable()
            .Include(x => x.Games.AsQueryable().WhereFiltered(filterType))
            .ThenInclude(x => x.Scores.AsQueryable().WhereFiltered(filterType))
            .Include(x => x.Games)
            .ThenInclude(x => x.Beatmap);

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
    /// Orders a <see cref="Match"/> query by the given <see cref="MatchesQuerySortType"/>
    /// </summary>
    public static IQueryable<Match> OrderBy(
        this IQueryable<Match> query,
        MatchesQuerySortType sortType,
        bool descending = false
    ) =>
        sortType switch
        {
            MatchesQuerySortType.OsuId => descending
                ? query.OrderByDescending(m => m.OsuId)
                : query.OrderBy(m => m.OsuId),
            MatchesQuerySortType.StartTime => descending
                ? query.OrderByDescending(m => m.StartTime)
                : query.OrderBy(m => m.StartTime),
            MatchesQuerySortType.EndTime => descending
                ? query.OrderByDescending(m => m.EndTime)
                : query.OrderBy(m => m.EndTime),
            _ => descending ? query.OrderByDescending(m => m.Id) : query.OrderBy(m => m.Id)
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
}
