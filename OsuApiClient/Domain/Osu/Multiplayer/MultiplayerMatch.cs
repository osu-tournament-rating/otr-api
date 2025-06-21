using AutoMapper;
using JetBrains.Annotations;
using OsuApiClient.Net.JsonModels.Osu.Multiplayer;

namespace OsuApiClient.Domain.Osu.Multiplayer;

/// <summary>
/// Represents a multiplayer lobby
/// </summary>
[AutoMap(typeof(MultiplayerMatchJsonModel))]
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class MultiplayerMatch : IModel
{
    /// <summary>
    /// Lobby info
    /// </summary>
    public MatchInfo Match { get; init; } = null!;

    /// <summary>
    /// A list of <see cref="MatchEvent"/>s that occurred
    /// </summary>
    public MatchEvent[] Events { get; internal set; } = [];

    /// <summary>
    /// A list of all <see cref="MatchUser"/>s that joined the lobby at any given point
    /// </summary>
    public MatchUser[] Users { get; internal set; } = [];

    /// <summary>
    /// Id of the first <see cref="MatchEvent"/> that occurred
    /// </summary>
    public long FirstEventId { get; init; }

    /// <summary>
    /// Id of the most recent <see cref="MatchEvent"/> that occurred
    /// </summary>
    public long LatestEventId { get; init; }

    /// <summary>
    /// No description
    /// </summary>
    public long? CurrentGameId { get; init; }
}
