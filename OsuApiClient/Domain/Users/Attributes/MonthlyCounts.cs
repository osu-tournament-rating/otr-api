using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using OsuApiClient.Net.JsonModels.Users.Attributes;

namespace OsuApiClient.Domain.Users.Attributes;

/// <summary>
/// Represents a statistic meant to be indexed by month
/// </summary>
[AutoMap(typeof(MonthlyCountsJsonModel))]
[SuppressMessage("ReSharper", "CommentTypo")]
public class MonthlyCounts : IModel
{
    /// <summary>
    /// Start date of the time range
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Target statistic as an numeric count
    /// </summary>
    public int Count { get; set; }
}
