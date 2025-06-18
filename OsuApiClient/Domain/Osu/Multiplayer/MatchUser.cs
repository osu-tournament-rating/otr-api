using AutoMapper;
using JetBrains.Annotations;
using OsuApiClient.Domain.Osu.Users;
using OsuApiClient.Domain.Osu.Users.Attributes;
using OsuApiClient.Net.JsonModels.Osu.Multiplayer;

namespace OsuApiClient.Domain.Osu.Multiplayer;

/// <summary>
/// Represents a user that was present in a <see cref="MultiplayerMatch"/>
/// </summary>
[AutoMap(typeof(MatchUserJsonModel))]
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class MatchUser : User
{
    /// <summary>
    /// Color of the user's username/profile highlight as a hex code
    /// </summary>
    public string? ProfileColour { get; init; }

    /// <summary>
    /// Information about the user's country
    /// </summary>
    public Country Country { get; init; } = null!;
}
