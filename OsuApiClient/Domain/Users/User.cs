using System.Diagnostics.CodeAnalysis;

namespace OsuApiClient.Domain.Users;

/// <summary>
/// Represents a user
/// </summary>
public class User : IModel
{
    /// <summary>
    /// Url of user's avatar
    /// </summary>
    public string AvatarUrl { get; init; } = null!;

    /// <summary>
    /// Two-letter code representing user's country
    /// </summary>
    public string CountryCode { get; init; } = null!;

    /// <summary>
    /// Identifier of the default <a href="https://osu.ppy.sh/docs/index.html#group">Group</a> the user belongs to
    /// </summary>
    public string? DefaultGroup { get; init; }

    /// <summary>
    /// Unique identifier for user
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Has this account been active in the last x months?
    /// </summary>
    /// <remarks>What defines "last x months" is not officially stated</remarks>
    public bool IsActive { get; init; }

    /// <summary>
    /// Is this a bot account?
    /// </summary>
    public bool IsBot { get; init; }

    /// <summary>
    /// Is the user's account deleted?
    /// </summary>
    /// <remarks>
    /// Undocumented, but in theory this is never true since fetching a deleted user would return a null response
    /// </remarks>
    public bool IsDeleted { get; init; }

    /// <summary>
    /// Is the user currently online? (either on Lazer or the website)
    /// </summary>
    [SuppressMessage("ReSharper", "CommentTypo")]
    public bool IsOnline { get; init; }

    /// <summary>
    ///	Does the user have an active supporter tag?
    /// </summary>
    public bool IsSupporter { get; init; }

    /// <summary>
    /// Last access time
    /// </summary>
    /// <remarks>Null if the user hides online presence</remarks>
    public DateTimeOffset? LastVisit { get; init; }

    /// <summary>
    /// Whether or not the user allows PM from other than friends
    /// </summary>
    public bool PmFriendsOnly { get; init; }

    /// <summary>
    /// Colour of username/profile highlight, hex code
    /// </summary>
    public string? ProfileColor { get; init; }

    /// <summary>
    /// User's display name
    /// </summary>
    public string Username { get; init; } = null!;
}
