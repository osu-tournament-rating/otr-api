using AutoMapper;
using AutoMapper.Configuration.Annotations;
using JetBrains.Annotations;
using OsuApiClient.Enums;
using OsuApiClient.Net.Deserialization.ValueConverters;
using OsuApiClient.Net.JsonModels.Osu.Multiplayer;

namespace OsuApiClient.Domain.Osu.Multiplayer;

/// <summary>
/// Represents details about a <see cref="MatchEvent"/>
/// </summary>
[AutoMap(typeof(MatchEventDetailJsonModel))]
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class MatchEventDetail : IModel
{
    /// <summary>
    /// Type of event
    /// </summary>
    [ValueConverter(typeof(MultiplayerEventTypeConverter))]
    public MultiplayerEventType Type { get; init; }

    /// <summary>
    /// Event text
    /// </summary>
    /// <remarks>When populated, this is always the lobby title</remarks>
    public string? Text { get; init; }
}
