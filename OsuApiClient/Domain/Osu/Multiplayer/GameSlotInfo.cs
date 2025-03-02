using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Common.Enums.Enums;
using OsuApiClient.Net.Deserialization.ValueConverters;
using OsuApiClient.Net.JsonModels.Osu.Multiplayer;

namespace OsuApiClient.Domain.Osu.Multiplayer;

/// <summary>
/// Represents info about what lobby slot a player was in for a <see cref="GameScore"/>
/// </summary>
[AutoMap(typeof(GameSlotInfoJsonModel))]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class GameSlotInfo : IModel
{
    /// <summary>
    /// Slot number
    /// </summary>
    public int Slot { get; init; }

    /// <summary>
    /// The <see cref="Common.Enums.Enums.Team"/> the player was on
    /// </summary>
    [ValueConverter(typeof(TeamConverter))]
    public Team Team { get; init; }

    /// <summary>
    /// Denotes if the player passed
    /// </summary>
    public bool Pass { get; init; }
}
