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

    /// <summary>
    /// Count of combos completed without the highest possible accuracy on every note
    /// </summary>
    /// <remarks>See <a href="https://osu.ppy.sh/wiki/en/Gameplay/Judgement/Katu">osu! Judgement - Katu</a></remarks>
    public int CountKatu { get; init; }

    /// <summary>
    /// Count of combos completed with the highest possible accuracy on every note
    /// </summary>
    /// <remarks>See <a href="https://osu.ppy.sh/wiki/en/Gameplay/Judgement/Geki">osu! Judgement - Geki</a></remarks>
    public int CountGeki { get; init; }

    public int CountMiss { get; init; }
}
