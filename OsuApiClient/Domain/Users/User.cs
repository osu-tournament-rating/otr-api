using AutoMapper;
using OsuApiClient.Net.JsonModels.Users;

namespace OsuApiClient.Domain.Users;

/// <summary>
/// Represents a user
/// </summary>
[AutoMap(typeof(UserJsonModel))]
public class User : IModel
{
    /// <summary>
    /// Url of user's avatar
    /// </summary>
    public string AvatarUrl { get; internal set; } = null!;

    /// <summary>
    /// Two-letter code representing user's country
    /// </summary>
    public string CountryCode { get; internal set; } = null!;

    /// <summary>
    /// Identifier of the default <a href="https://osu.ppy.sh/docs/index.html#group">Group</a> the user belongs to
    /// </summary>
    public string? DefaultGroup { get; internal set; }

    /// <summary>
    /// Unique identifier for user
    /// </summary>
    public long Id { get; internal set; }

    /// <summary>
    /// Has this account been active in the last x months?
    /// </summary>
    /// <remarks>What defines "last x months" is not officially stated</remarks>
    public bool IsActive { get; internal set; }

    /// <summary>
    /// Is this a bot account?
    /// </summary>
    public bool IsBot { get; internal set; }

    /// <summary>
    /// Is the user's account deleted?
    /// </summary>
    /// <remarks>
    /// Undocumented, but in theory this is never true since fetching a deleted user would return a null response
    /// </remarks>
    public bool IsDeleted { get; internal set; }

    /// <summary>
    /// Is the user currently online? (either on Lazer or the website)
    /// </summary>
    public bool IsOnline { get; internal set; }

    /// <summary>
    ///	Does the user have an active supporter tag?
    /// </summary>
    public bool IsSupporter { get; internal set; }

    /// <summary>
    /// Last access time
    /// </summary>
    /// <remarks>Null if the user hides online presence</remarks>
    public DateTimeOffset? LastVisit { get; internal set; }

    /// <summary>
    /// Whether or not the user allows PM from other than friends
    /// </summary>
    public bool PmFriendsOnly { get; internal set; }

    /// <summary>
    /// Colour of username/profile highlight, hex code
    /// </summary>
    public string? ProfileColour { get; internal set; }

    /// <summary>
    /// User's display name
    /// </summary>
    public string Username { get; internal set; } = null!;
}
