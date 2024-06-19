using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using OsuApiClient.Net.JsonModels.Multiplayer;

namespace OsuApiClient.Domain.Multiplayer;

/// <summary>
/// Represents multiplayer lobby information
/// </summary>
[AutoMap(typeof(MatchInfoJsonModel))]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class MatchInfo : IModel
{
    /// <summary>
    /// Id of the lobby (mp id)
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Timestamp for the start of the lobby
    /// </summary>
    public DateTimeOffset StartTime { get; init; }

    /// <summary>
    /// Timestamp for the end of the lobby
    /// </summary>
    public DateTimeOffset? EndTime { get; init; }

    /// <summary>
    /// Lobby title
    /// </summary>
    public string Name { get; init; } = null!;
}
