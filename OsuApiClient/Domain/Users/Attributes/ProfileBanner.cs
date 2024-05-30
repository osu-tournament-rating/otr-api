using AutoMapper;
using OsuApiClient.Net.JsonModels.Users.Attributes;

namespace OsuApiClient.Domain.Users.Attributes;

/// <summary>
/// Represents a user tournament profile banner
/// </summary>
[AutoMap(typeof(ProfileBannerJsonModel))]
public class ProfileBanner : IModel
{
    /// <summary>
    /// Id of the banner
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Id of the tournament the banner is from
    /// </summary>
    public int TournamentId { get; set; }

    /// <summary>
    /// Url of the image
    /// </summary>
    public string? Image { get; set; }

    /// <summary>
    /// Url of the image in 2x resolution
    /// </summary>
    public string? Image2X { get; set; }
}
