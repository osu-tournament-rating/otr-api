using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;
using Database.Queries.Filters;
using Microsoft.EntityFrameworkCore;

namespace Database.Queries.Extensions;

public static class TournamentQueryExtensions
{
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
    /// Filters a <see cref="Tournament"/> query for those with a <see cref="VerificationStatus"/>
    /// of <see cref="VerificationStatus.Verified"/>
    /// </summary>
    public static IQueryable<Tournament> WhereVerified(this IQueryable<Tournament> query) =>
        query.Where(x => x.VerificationStatus == VerificationStatus.Verified);

    /// <summary>
    /// Filters a <see cref="Tournament"/> query for those with a <see cref="TournamentProcessingStatus"/>
    /// of <see cref="TournamentProcessingStatus.Done"/>
    /// </summary>
    public static IQueryable<Tournament> WhereProcessingCompleted(this IQueryable<Tournament> query) =>
        query.Where(x => x.ProcessingStatus == TournamentProcessingStatus.Done);
}
