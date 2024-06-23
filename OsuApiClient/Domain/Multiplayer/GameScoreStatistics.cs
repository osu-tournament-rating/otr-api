using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Database.Entities.Interfaces;
using OsuApiClient.Net.JsonModels.Multiplayer;

namespace OsuApiClient.Domain.Multiplayer;

/// <summary>
/// Represents a set of accuracy values for a <see cref="GameScore"/>
/// </summary>
[AutoMap(typeof(GameScoreStatisticsJsonModel))]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class GameScoreStatistics : IModel, IScoreStatistics
{
    public int Count50 { get; init; }

    public int Count100 { get; init; }

    public int Count300 { get; init; }

    public int CountKatu { get; init; }

    public int CountGeki { get; init; }

    public int CountMiss { get; init; }
}
