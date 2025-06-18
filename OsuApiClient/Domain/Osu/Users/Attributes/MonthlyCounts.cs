using AutoMapper;
using JetBrains.Annotations;
using OsuApiClient.Net.JsonModels.Osu.Users.Attributes;

namespace OsuApiClient.Domain.Osu.Users.Attributes;

/// <summary>
/// Represents a statistic meant to be indexed by month
/// </summary>
[AutoMap(typeof(MonthlyCountsJsonModel))]
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class MonthlyCounts : IModel
{
    /// <summary>
    /// Start date of the time range
    /// </summary>
    public DateTime StartDate { get; init; }

    /// <summary>
    /// Target statistic as an numeric count
    /// </summary>
    public int Count { get; init; }
}
