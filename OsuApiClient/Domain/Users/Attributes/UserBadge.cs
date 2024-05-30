using AutoMapper;
using OsuApiClient.Net.JsonModels.Users.Attributes;

namespace OsuApiClient.Domain.Users.Attributes;

/// <summary>
/// Represents a user badge
/// </summary>
[AutoMap(typeof(UserBadgeJsonModel))]
public class UserBadge : IModel
{
    /// <summary>
    /// Timestamp for when the badge was granted
    /// </summary>
    public DateTimeOffset AwardedAt { get; set; }

    /// <summary>
    /// Description of the badge
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Url for the badge's image
    /// </summary>
    public string ImageUrl { get; set; } = null!;

    /// <summary>
    /// Url for the badge's image in 2x resolution
    /// </summary>
    public string Image2XUrl { get; set; } = null!;

    /// <summary>
    /// Url to the badge's information
    /// </summary>
    public string Url { get; set; } = null!;
}
