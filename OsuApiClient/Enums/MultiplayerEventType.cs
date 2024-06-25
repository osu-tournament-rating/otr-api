using OsuApiClient.Domain.Osu.Multiplayer;

namespace OsuApiClient.Enums;

/// <summary>
/// Describes the type of a <see cref="MatchEvent"/>
/// </summary>
public enum MultiplayerEventType
{
    /// <summary>
    /// Unknown event type
    /// </summary>
    Unknown,

    /// <summary>
    /// Lobby was created
    /// </summary>
    MatchCreated,

    /// <summary>
    /// Lobby was closed
    /// </summary>
    MatchDisbanded,

    /// <summary>
    /// A player has joined the lobby
    /// </summary>
    PlayerJoined,

    /// <summary>
    /// A player has left the lobby
    /// </summary>
    PlayerLeft,

    /// <summary>
    /// A game was played
    /// </summary>
    Game
}
