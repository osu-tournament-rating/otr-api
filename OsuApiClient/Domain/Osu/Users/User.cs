using System.Diagnostics.CodeAnalysis;

namespace OsuApiClient.Domain.Osu.Users;

/// <summary>
/// Represents a user
/// </summary>
public class User : IModel
{
    /// <summary>
    /// Url of the user's avatar
    /// </summary>
    public string AvatarUrl { get; init; } = null!;

    /// <summary>
    /// Two-letter code representing the user's country
    /// </summary>
    public string CountryCode { get; init; } = null!;

    /// <summary>
    /// Identifier of the default <a href="https://osu.ppy.sh/docs/index.html#group">Group</a> the user belongs to
    /// </summary>
    public string? DefaultGroup { get; init; }

    /// <summary>
    /// Unique identifier for the user
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Denotes if the user has been active in the last x months
    /// </summary>
    /// <remarks>What defines "last x months" is not documented</remarks>
    public bool IsActive { get; init; }

    /// <summary>
    /// Denotes if the user's account is a bot account
    /// </summary>
    public bool IsBot { get; init; }

    /// <summary>
    /// Denotes if the user's account is deleted
    /// </summary>
    /// <remarks>
    /// Undocumented, but in theory this is never true since fetching a deleted user would return a null response
    /// </remarks>
    public bool IsDeleted { get; init; }

    /// <summary>
    /// Denotes if the user is currently online (either on Lazer or the website)
    /// </summary>
    [SuppressMessage("ReSharper", "CommentTypo")]
    public bool IsOnline { get; init; }

    /// <summary>
    ///	Denotes if the user has an active supporter tag
    /// </summary>
    public bool IsSupporter { get; init; }

    /// <summary>
    /// Timestamp of last access to any osu! services
    /// </summary>
    /// <remarks>Null if the user hides online presence</remarks>
    public DateTimeOffset? LastVisit { get; init; }

    /// <summary>
    /// Denotes if the user allows PMs from users other than friends
    /// </summary>
    public bool PmFriendsOnly { get; init; }

    /// <summary>
    /// Color of the user's username/profile highlight as a hex code
    /// </summary>
    public string? ProfileColor { get; init; }

    /// <summary>
    /// User's display name
    /// </summary>
    public string Username { get; init; } = null!;
}
