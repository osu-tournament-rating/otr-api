using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;

namespace Database.Utilities.Extensions;

public static class CommonQueryExtensions
{
    /// <summary>
    /// Gets the desired "page" of a query
    /// </summary>
    /// <param name="limit">Page size</param>
    /// <param name="page">Desired page (zero-indexed)</param>
    public static IQueryable<T> Page<T>(this IQueryable<T> query, int limit, int page) =>
        query.AsQueryable().Skip(limit * page).Take(limit);

    /// <summary>
    /// Applies common query filters and ensures verified data is returned
    /// </summary>
    /// <param name="query">Query</param>
    /// <param name="ruleset">Ruleset</param>
    /// <param name="dateMin">Minimum inclusion date</param>
    /// <param name="dateMax">Maximum inclusion date</param>
    /// <returns>The filtered query</returns>
    public static IQueryable<PlayerMatchStats> ApplyCommonFilters(this IQueryable<PlayerMatchStats> query,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null)
    {
        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;
        return query.Where(pms => pms.Match.VerificationStatus == VerificationStatus.Verified && pms.Match.Tournament.Ruleset == ruleset &&
                                                                           pms.Match.StartTime >= dateMin && pms.Match.StartTime <= dateMax);
    }

    /// <inheritdoc cref="ApplyCommonFilters(System.Linq.IQueryable{Database.Entities.PlayerMatchStats},Database.Enums.Ruleset,System.DateTime?,System.DateTime?)"/>
    public static IQueryable<GameScore> ApplyCommonFilters(this IQueryable<GameScore> query,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null)
    {
        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;
        return query.Where(gs => gs.VerificationStatus == VerificationStatus.Verified && gs.Game.Match.Tournament.Ruleset == ruleset &&
                                                                   gs.Game.Match.StartTime >= dateMin && gs.Game.Match.StartTime <= dateMax);
    }
}
