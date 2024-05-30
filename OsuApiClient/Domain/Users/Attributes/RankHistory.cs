using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using OsuApiClient.Net.JsonModels.Users.Attributes;

namespace OsuApiClient.Domain.Users.Attributes;

/// <summary>
/// Represents the history of a user's rank for a ruleset
/// </summary>
[AutoMap(typeof(RankHistoryJsonModel))]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class RankHistory : IModel
{
    /// <summary>
    /// The mode the history data is for
    /// </summary>
    public string Mode { get; init; } = null!;

    /// <summary>
    /// A collection of numbers representing the user's rank ordered by day
    /// </summary>
    /// <remarks>This collection should always have a size of 90</remarks>
    public long[] Data { get; init; } = null!;
}
