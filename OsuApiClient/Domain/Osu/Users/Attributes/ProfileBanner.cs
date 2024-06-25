using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using OsuApiClient.Net.JsonModels.Osu.Users.Attributes;

namespace OsuApiClient.Domain.Osu.Users.Attributes;

/// <summary>
/// Represents a user tournament profile banner
/// </summary>
[AutoMap(typeof(ProfileBannerJsonModel))]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class ProfileBanner : IModel
{
    /// <summary>
    /// Id of the banner
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Id of the tournament the banner is from
    /// </summary>
    public int TournamentId { get; init; }

    /// <summary>
    /// Url of the image
    /// </summary>
    public string? Image { get; init; }

    /// <summary>
    /// Url of the image in 2x resolution
    /// </summary>
    public string? Image2X { get; init; }
}
