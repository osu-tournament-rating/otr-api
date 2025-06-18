using AutoMapper;
using JetBrains.Annotations;
using OsuApiClient.Net.JsonModels.Osu.Multiplayer;

namespace OsuApiClient.Domain.Osu.Multiplayer;

/// <summary>
/// Represents an event that occurred in a <see cref="MultiplayerMatch"/>
/// </summary>
[AutoMap(typeof(MatchEventJsonModel))]
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class MatchEvent : IModel
{
    /// <summary>
    /// Id of the event
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Event details
    /// </summary>
    public MatchEventDetail Detail { get; init; } = null!;

    /// <summary>
    /// Timestamp of the event
    /// </summary>
    public DateTime Timestamp { get; init; }

    /// <summary>
    /// No description
    /// </summary>
    public long? UserId { get; init; }

    /// <summary>
    /// Game details. Populated only if <see cref="Detail"/>.<see cref="MatchEventDetail.Type"/>
    /// is <see cref="Enums.MultiplayerEventType.Game"/>
    /// </summary>
    public MultiplayerGame? Game { get; init; }
}
