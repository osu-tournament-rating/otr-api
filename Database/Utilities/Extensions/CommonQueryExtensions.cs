using System.Linq.Expressions;
using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database.Utilities.Extensions;

public static class CommonQueryExtensions
{
    /// <summary>
    /// Gets the desired "page" of a query
    /// </summary>
    /// <param name="page">Desired page (one-indexed)</param>
    /// <param name="pageSize">Page size</param>
    public static IQueryable<T> Page<T>(this IQueryable<T> query, int page, int pageSize) =>
        query.AsQueryable().Skip(pageSize * (page - 1)).Take(pageSize);

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

    /// <inheritdoc cref="ApplyCommonFilters(System.Linq.IQueryable{Database.Entities.PlayerMatchStats},Ruleset,System.DateTime?,System.DateTime?)"/>
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

    /// <inheritdoc cref="ApplyCommonFilters(System.Linq.IQueryable{Database.Entities.PlayerMatchStats},Ruleset,System.DateTime?,System.DateTime?)"/>
    public static IQueryable<PlayerTournamentStats> ApplyCommonFilters(this IQueryable<PlayerTournamentStats> query,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null)
    {
        dateMin ??= DateTime.MinValue;
        dateMax ??= DateTime.MaxValue;
        return query.Where(pts => pts.Tournament.Ruleset == ruleset &&
                                  pts.Tournament.StartTime >= dateMin &&
                                  pts.Tournament.StartTime <= dateMax);
    }

    /// <summary>
    /// Materializes a query into a dictionary where the key is a property of <typeparamref name="TEntity"/> selected by
    /// <paramref name="propertySelector"/>, and the value is the count of corresponding <typeparamref name="TEntity"/> items
    /// </summary>
    /// <typeparam name="TEntity">The type of the queried entities</typeparam>
    /// <typeparam name="TProp">The type of the property to count by</typeparam>
    /// <param name="query">The query to materialize</param>
    /// <param name="propertySelector">An expression that selects the property to count by</param>
    /// <returns>A dictionary mapping each unique <typeparamref name="TProp"/> value to
    /// the number of <typeparamref name="TEntity"/> items with that property value</returns>
    public static async Task<Dictionary<TProp, int>> ToCountStatisticsDictionaryAsync<TEntity, TProp>(
        this IQueryable<TEntity> query,
        Expression<Func<TEntity, TProp>> propertySelector)
        where TProp : notnull =>
        await query
            .GroupBy(
                propertySelector,
                (x, y) => new { Prop = x, Count = y.Count() })
            .ToDictionaryAsync(x => x.Prop, x => x.Count);
}
