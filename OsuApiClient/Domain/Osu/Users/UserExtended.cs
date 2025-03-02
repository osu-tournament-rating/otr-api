using System.Diagnostics.CodeAnalysis;
using Common.Enums.Enums;
using OsuApiClient.Domain.Osu.Users.Attributes;

namespace OsuApiClient.Domain.Osu.Users;

/// <summary>
/// Represents a user. Extends <see cref="User"/> with additional optional attributes
/// </summary>
public class UserExtended : User
{
    /// <summary>
    /// User's cover url
    /// </summary>
    public string CoverUrl { get; init; } = null!;

    /// <summary>
    /// User's Discord handle
    /// </summary>
    public string? Discord { get; init; }

    /// <summary>
    /// Denotes whether the user purchased a supporter tag at any point
    /// </summary>
    public bool HasSupported { get; init; }

    /// <summary>
    /// User's interests
    /// </summary>
    public string? Interests { get; init; }

    /// <summary>
    /// Date the user joined osu!
    /// </summary>
    public DateTimeOffset JoinDate { get; init; }

    /// <summary>
    /// User's location
    /// </summary>
    public string? Location { get; init; }

    /// <summary>
    /// Max number of users the user can block
    /// </summary>
    public int MaxBlocks { get; init; }

    /// <summary>
    /// Max number of friends the user can have
    /// </summary>
    public int MaxFriends { get; init; }

    /// <summary>
    /// User's occupation
    /// </summary>
    public string? Occupation { get; init; }

    /// <summary>
    /// User's default ruleset
    /// </summary>
    public Ruleset Ruleset { get; init; }

    /// <summary>
    /// User's play style
    /// </summary>
    /// <example>
    /// Contains any of the following:
    /// "mouse", "keyboard", "tablet", "touch"
    /// </example>
    public string[] PlayStyle { get; init; } = [];

    /// <summary>
    /// Number of posts the user has made on the osu! forums
    /// </summary>
    public int PostCount { get; init; }

    /// <summary>
    /// Order of the sections on the user's page
    /// </summary>
    /// <example>
    /// Contains the following in order of placement:
    /// "me", "top_ranks", "recent_activity", "historical", "beatmaps", "medals", "kudosu"
    /// </example>
    [SuppressMessage("ReSharper", "CommentTypo")]
    public string[] ProfileOrder { get; init; } = [];

    /// <summary>
    /// User's title
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// User's title url
    /// </summary>
    public string? TitleUrl { get; init; }

    /// <summary>
    /// User's twitter handle
    /// </summary>
    public string? Twitter { get; init; }

    /// <summary>
    /// User's website
    /// </summary>
    public string? Website { get; init; }

    /// <summary>
    /// Information about the user's country
    /// </summary>
    public Country Country { get; init; } = null!;

    /// <summary>
    /// User's cover
    /// </summary>
    public Cover Cover { get; init; } = null!;

    /// <summary>
    /// Denotes whether the user is currently restricted
    /// </summary>
    /// <remarks>Optional attribute. Only included if the user is the currently authorized user</remarks>
    public bool? IsRestricted { get; init; }

    /// <summary>
    /// Information about the user's Kudosu
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "CommentTypo")]
    public Kudosu? Kudosu { get; init; }

    # region Optional Attributes

    /// <summary>
    /// A collection of information about disciplinary actions taken against the user
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public UserAccountHistory[] AccountHistory { get; init; } = [];

    /// <summary>
    /// A collection of information about the user's achievements
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public UserAchievement[] Achievements { get; init; } = [];

    /// <summary>
    /// User's active tournament banner
    /// </summary>
    /// <remarks>Optional attribute. Deprecated in favor of <see cref="ActiveTournamentBanners"/></remarks>
    public ProfileBanner? ActiveTournamentBanner { get; init; }

    /// <summary>
    /// A collection of the user's active tournament banners
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public ProfileBanner[] ActiveTournamentBanners { get; init; } = [];

    /// <summary>
    /// A collection of the user's badges
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public UserBadge[] Badges { get; init; } = [];

    /// <summary>
    /// Number of beatmaps played by the user
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public long BeatmapPlayCount { get; init; }

    // blocks

    /// <summary>
    /// Number of comments the user has made on the osu! website
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public int CommentsCount { get; init; }

    /// <summary>
    /// Number of the user's favorite beatmaps
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public int FavouriteBeatmapsCount { get; init; }

    // follow_user_mapping

    /// <summary>
    /// Number of followers the user has
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public int FollowerCount { get; init; }

    // friends

    /// <summary>
    /// Count of graveyard beatmaps submitted by the user
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public int GraveyardBeatmapsCount { get; init; }

    /// <summary>
    /// A collection of groups the user belongs to
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public UserGroup[] Groups { get; init; } = [];

    /// <summary>
    /// Count of guest beatmaps submitted by the user
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public int GuestBeatmapsCount { get; init; }

    /// <summary>
    /// Count of loved beatmaps submitted by the user
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public int LovedBeatmapsCount { get; init; }

    /// <summary>
    /// Count of mapping followers the user has
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public int MappingFollowerCount { get; init; }

    /// <summary>
    /// A collection of the user's beatmap play counts by month
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public MonthlyCounts[] MonthlyPlayCounts { get; init; } = [];

    /// <summary>
    /// Raw HTML containing the user's page
    /// </summary>
    // public string? Page { get; set; }

    /// <summary>
    /// Count of pending beatmaps submitted by the user
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public int PendingBeatmapsCount { get; init; }

    /// <summary>
    /// A collection of the user's past usernames
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public string[] PreviousUsernames { get; init; } = [];

    /// <summary>
    /// Information about the user's highest recorded global rank
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public HighestRank? HighestRank { get; init; }

    /// <summary>
    /// Count of ranked or approved beatmaps submitted by the user
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public int RankedAndApprovedBeatmapsCount { get; init; }

    /// <summary>
    /// Information about the user's rank by day for the last 90 days
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public RankHistory? RankHistory { get; init; }

    /// <summary>
    /// Count of ranked beatmaps submitted by the user
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public int RankedBeatmapsCount { get; init; }

    /// <summary>
    /// A collection of the user's replay watch counts by month
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public MonthlyCounts[] ReplaysWatchedCounts { get; init; } = [];

    /// <summary>
    /// No description
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public int BestScoresCount { get; init; }

    /// <summary>
    /// No description
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public int FirstScoresCount { get; init; }

    /// <summary>
    /// Count of the user's recent scores
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public int RecentScoresCount { get; init; }

    /// <summary>
    /// Count of scores the user has pinned
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public int PinnedScoresCount { get; init; }

    // session_verified

    /// <summary>
    /// A summary of various gameplay statistics specific to the <see cref="Ruleset"/>
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public UserStatistics? Statistics { get; init; }

    // statistics_ruleset

    // support_level

    /// <summary>
    /// Count of unranked beatmaps submitted by the user
    /// </summary>
    /// <remarks>Optional attribute</remarks>
    public int UnrankedBeatmapsCount { get; init; }

    // unread_pm_count

    # endregion
}
