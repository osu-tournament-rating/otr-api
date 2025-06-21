using AutoMapper;
using Database.Entities.Interfaces;
using JetBrains.Annotations;
using OsuApiClient.Net.JsonModels.Osu.Multiplayer;

namespace OsuApiClient.Domain.Osu.Multiplayer;

/// <summary>
/// Represents a set of accuracy values for a <see cref="GameScore"/>
/// </summary>
[AutoMap(typeof(GameScoreStatisticsJsonModel))]
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class GameScoreStatistics : IModel, IScoreStatistics
{
    public int Count50 { get; init; }

    public int Count100 { get; init; }

    public int Count300 { get; init; }

    public int CountKatu { get; init; }

    public int CountGeki { get; init; }

    public int CountMiss { get; init; }
}
