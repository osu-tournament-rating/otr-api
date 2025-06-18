using AutoMapper;
using JetBrains.Annotations;
using OsuApiClient.Net.JsonModels.Osu.Users.Attributes;

namespace OsuApiClient.Domain.Osu.Users.Attributes;

/// <summary>
/// Represents a user badge
/// </summary>
[AutoMap(typeof(UserBadgeJsonModel))]
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class UserBadge : IModel
{
    /// <summary>
    /// Timestamp for when the badge was granted
    /// </summary>
    public DateTimeOffset AwardedAt { get; init; }

    /// <summary>
    /// Description of the badge
    /// </summary>
    public string Description { get; init; } = null!;

    /// <summary>
    /// Url for the badge's image
    /// </summary>
    public string ImageUrl { get; init; } = null!;

    /// <summary>
    /// Url for the badge's image in 2x resolution
    /// </summary>
    public string Image2XUrl { get; init; } = null!;

    /// <summary>
    /// Url to the badge's information
    /// </summary>
    public string Url { get; init; } = null!;
}
