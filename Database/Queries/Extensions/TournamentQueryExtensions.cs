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
            TournamentsQuerySortType.StartTime => descending
                ? query.OrderByDescending(t => t.StartTime)
                : query.OrderBy(t => t.StartTime),
            TournamentsQuerySortType.EndTime => descending
                ? query.OrderByDescending(t => t.EndTime)
                : query.OrderBy(t => t.EndTime),
            _ => descending ? query.OrderByDescending(t => t.Id) : query.OrderBy(t => t.Id)
        };

    /// <summary>
    /// Includes all navigations on a <see cref="Tournament"/> query
    /// </summary>
    /// <param name="filtered">
    /// Denotes if the included navigations should be limited to only entities that have a
    /// <see cref="VerificationStatus"/> of <see cref="VerificationStatus.Verified"/> and have completed processing
    /// </param>
    public static IQueryable<Tournament> IncludeAll(this IQueryable<Tournament> query, bool filtered = true)
    {
        query = query.AsSplitQuery();

        if (filtered)
        {
            query = query
                .Include(t => t.Matches.Where(m =>
                    m.VerificationStatus == VerificationStatus.Verified
                    && m.ProcessingStatus == MatchProcessingStatus.Done))
                .ThenInclude(m => m.Games.Where(g =>
                    g.VerificationStatus == VerificationStatus.Verified
                    && g.ProcessingStatus == GameProcessingStatus.Done))
                .ThenInclude(g => g.Scores.Where(s =>
                    s.VerificationStatus == VerificationStatus.Verified
                    && s.ProcessingStatus == ScoreProcessingStatus.Done
                ));
        }
        else
        {
            query = query
                .Include(t => t.Matches)
                .ThenInclude(m => m.Games)
                .ThenInclude(g => g.Scores);
        }

        return query
            .Include(t => t.PlayerTournamentStats)
            .Include(t => t.Matches)
            .ThenInclude(m => m.WinRecord)
            .Include(t => t.Matches)
            .ThenInclude(m => m.PlayerMatchStats)
            .Include(t => t.Matches)
            .ThenInclude(m => m.PlayerRatingAdjustments)
            .Include(t => t.Matches)
            .ThenInclude(m => m.Games)
            .ThenInclude(g => g.Beatmap)
            .Include(t => t.Matches)
            .ThenInclude(m => m.Games)
            .ThenInclude(g => g.WinRecord);
    }
}
