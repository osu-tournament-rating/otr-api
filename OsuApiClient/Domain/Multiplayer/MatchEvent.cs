using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using OsuApiClient.Net.JsonModels.Multiplayer;

namespace OsuApiClient.Domain.Multiplayer;

/// <summary>
/// Represents an event that occurred in a <see cref="MultiplayerMatch"/>
/// </summary>
[AutoMap(typeof(MatchEventJsonModel))]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
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
    /// Game details. Populated only if <see cref="Detail"/>.Type is <see cref="Enums.MultiplayerEventType.Game"/>
    /// </summary>
    public MultiplayerGame? Game { get; init; }
}
