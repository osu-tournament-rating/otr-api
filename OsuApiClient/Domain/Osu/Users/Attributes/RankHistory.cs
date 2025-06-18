using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Common.Enums;
using JetBrains.Annotations;
using OsuApiClient.Net.Deserialization.ValueConverters;
using OsuApiClient.Net.JsonModels.Osu.Users.Attributes;

namespace OsuApiClient.Domain.Osu.Users.Attributes;

/// <summary>
/// Represents the history of a user's rank for a ruleset
/// </summary>
[AutoMap(typeof(RankHistoryJsonModel))]
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class RankHistory : IModel
{
    /// <summary>
    /// The ruleset the history data is for
    /// </summary>
    [ValueConverter(typeof(RulesetConverter))]
    [SourceMember(nameof(RankHistoryJsonModel.Mode))]
    public Ruleset Ruleset { get; init; }

    /// <summary>
    /// A collection of numbers representing the user's rank ordered by day
    /// </summary>
    /// <remarks>This collection should always have a size of 90</remarks>
    public long[] Data { get; init; } = null!;
}
