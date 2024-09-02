using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;
using Database.Queries.Enums;
using Database.Queries.Filters;
using Microsoft.EntityFrameworkCore;

namespace Database.Queries.Extensions;

public static class TournamentQueryExtensions
{
    /// <summary>
    /// Filters a <see cref="Tournament"/> query based on the given <see cref="TournamentsQueryFilter"/>
    /// </summary>
    public static IQueryable<Tournament> WhereFiltered(this IQueryable<Tournament> query, TournamentsQueryFilter filter)
    {
        if (filter.Ruleset.HasValue)
        {
            query = query.WhereRuleset(filter.Ruleset.Value);
        }

        if (!string.IsNullOrEmpty(filter.Name))
        {
            query = query.WhereName(filter.Name);
        }

        if (filter.LobbySize.HasValue)
        {
            query = query.WhereLobbySize(filter.LobbySize.Value);
        }

        if (filter.RankRangeLowerBound.HasValue)
        {
            query = query.WhereRankRange(filter.RankRangeLowerBound.Value);
        }

        if (filter.VerificationStatus.HasValue)
        {
            query = query.WhereVerificationStatus(filter.VerificationStatus.Value);
        }

        if (filter.RejectionReason.HasValue)
        {
            query = query.WhereRejectionReason(filter.RejectionReason.Value);
        }

        if (filter.ProcessingStatus.HasValue)
        {
            query = query.WhereProcessingStatus(filter.ProcessingStatus.Value);
        }

        if (filter.SubmittedBy.HasValue)
        {
            query = query.WhereSubmittedBy(filter.SubmittedBy.Value);
        }

        if (filter.VerifiedBy.HasValue)
        {
            query = query.WhereVerifiedBy(filter.VerifiedBy.Value);
        }

        query = query.OrderBy(filter.Sort ?? TournamentsQuerySortType.Id, filter.SortDescending ?? false);

        return query;
    }

    /// <summary>
    /// Filters a <see cref="Tournament"/> query for those with a <see cref="Tournament.Name"/>
    /// or <see cref="Tournament.Abbreviation"/> that partially matches the given name
    /// </summary>
    public static IQueryable<Tournament> WhereName(this IQueryable<Tournament> query, string name)
    {
        name = name.Replace("_", @"\_");
        return query.Where(t =>
            EF.Functions.ILike(t.Name, $"%{name}%", @"\")
            || EF.Functions.ILike(t.Abbreviation, $"%{name}%", @"\")
        );
    }

    /// <summary>
    /// Filters a <see cref="Tournament"/> query for those played in the given <see cref="Ruleset"/>
    /// </summary>
    public static IQueryable<Tournament> WhereRuleset(this IQueryable<Tournament> query, Ruleset ruleset) =>
        query.Where(t => t.Ruleset == ruleset);

    /// <summary>
    /// Filters a <see cref="Tournament"/> query for those with a <see cref="Tournament.LobbySize"/>
    /// matching the given lobby size
    /// </summary>
    public static IQueryable<Tournament> WhereLobbySize(this IQueryable<Tournament> query, int lobbySize) =>
        query.Where(t => t.LobbySize == lobbySize);

    /// <summary>
    /// Filters a <see cref="Tournament"/> query for those with a <see cref="Tournament.RankRangeLowerBound"/>
    /// matching the given rank range
    /// </summary>
    public static IQueryable<Tournament> WhereRankRange(this IQueryable<Tournament> query, int rangeRange) =>
        query.Where(t => t.RankRangeLowerBound == rangeRange);

    /// <summary>
    /// Filters a <see cref="Tournament"/> query for those with a <see cref="Tournament.VerificationStatus"/>
    /// matching the given verification status
    /// </summary>
    public static IQueryable<Tournament> WhereVerificationStatus(
        this IQueryable<Tournament> query,
        VerificationStatus verificationStatus
    ) =>
        query.Where(t => t.VerificationStatus == verificationStatus);

    /// <summary>
    /// Filters a <see cref="Tournament"/> query for those with a <see cref="Tournament.RejectionReason"/>
    /// matching the given rejection reason
    /// </summary>
    public static IQueryable<Tournament> WhereRejectionReason(
        this IQueryable<Tournament> query,
        TournamentRejectionReason rejectionReason
    ) =>
        query.Where(t => t.RejectionReason == rejectionReason);

    /// <summary>
    /// Filters a <see cref="Tournament"/> query for those with a <see cref="Tournament.ProcessingStatus"/>
    /// matching the given processing status
    /// </summary>
    public static IQueryable<Tournament> WhereProcessingStatus(
        this IQueryable<Tournament> query,
        TournamentProcessingStatus processingStatus
    ) =>
        query.Where(t => t.ProcessingStatus == processingStatus);

    /// <summary>
    /// Filters a <see cref="Tournament"/> query for those with a <see cref="Tournament.SubmittedByUser"/>
    /// with an <see cref="User.Id"/> matching the given id
    /// </summary>
    public static IQueryable<Tournament> WhereSubmittedBy(this IQueryable<Tournament> query, int id) =>
        query.Where(t => t.SubmittedByUserId == id);

    /// <summary>
    /// Filters a <see cref="Tournament"/> query for those with a <see cref="Tournament.VerifiedByUser"/>
    /// with an <see cref="User.Id"/> matching the given id
    /// </summary>
    public static IQueryable<Tournament> WhereVerifiedBy(this IQueryable<Tournament> query, int id) =>
        query.Where(t => t.VerifiedByUserId == id);

    /// <summary>
    /// Orders a <see cref="Tournament"/> query by the given <see cref="TournamentsQuerySortType"/>
    /// </summary>
    public static IQueryable<Tournament> OrderBy(
        this IQueryable<Tournament> query,
        TournamentsQuerySortType sortType,
        bool descending = false
    ) =>
        sortType switch
        {
            _ => descending ? query.OrderByDescending(t => t.Id) : query.OrderBy(t => t.Id)
        };
}
