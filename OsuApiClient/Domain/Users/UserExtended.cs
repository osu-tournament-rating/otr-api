using System.Diagnostics.CodeAnalysis;
using OsuApiClient.Domain.Users.Attributes;

namespace OsuApiClient.Domain.Users;

/// <summary>
/// Represents a user. Extends <see cref="User"/> with additional optional attributes
/// </summary>
public class UserExtended : User
{
    /// <summary>
    /// A collection of the user's past usernames
    /// </summary>
    public string[] PreviousUsernames { get; init; } = [];

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
    /// Date the user joined osu!
    /// </summary>
    public DateTimeOffset JoinDate { get; init; }

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
    public string? PlayMode { get; init; }

    /// <summary>
    /// User's Discord handle
    /// </summary>
    public string? Discord { get; init; }

    /// <summary>
    /// User's interests
    /// </summary>
    public string? Interests { get; init; }

    /// <summary>
    /// User's location
    /// </summary>
    public string? Location { get; init; }

    /// <summary>
    /// User's play style
    /// </summary>
    /// <example>
    /// Contains any of the following:
    /// "mouse", "keyboard", "tablet", "touch"
    /// </example>
    public string[] PlayStyle { get; init; } = [];

    /// <summary>
    /// Information about the user's country
    /// </summary>
    public Country? Country { get; init; }

    /// <summary>
    /// User's cover
    /// </summary>
    public Cover? Cover { get; init; }

    /// <summary>
    /// User's cover url
    /// </summary>
    public string CoverUrl { get; init; } = null!;

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
    /// Raw HTML containing the user's page
    /// </summary>
    // public string? Page { get; set; }

    /// <summary>
    /// Denotes whether the user purchased a supporter tag at any point
    /// </summary>
    public bool HasSupported { get; init; }

    /// <summary>
    /// No description
    /// </summary>
    public int BestScoresCount { get; init; }

    /// <summary>
    /// No description
    /// </summary>
    public int FirstScoresCount { get; init; }

    /// <summary>
    /// Count of scores the user has pinned
    /// </summary>
    public int PinnedScoresCount { get; init; }

    /// <summary>
    /// Count of the user's recent scores
    /// </summary>
    public int RecentScoresCount { get; init; }

    /// <summary>
    /// Number of beatmaps played by the user
    /// </summary>
    public long BeatmapPlayCount { get; init; }

    /// <summary>
    /// Number of the user's favorite beatmaps
    /// </summary>
    public int FavouriteBeatmapsCount { get; init; }

    /// <summary>
    /// Number of followers the user has
    /// </summary>
    public int FollowerCount { get; init; }

    /// <summary>
    /// Count of mapping followers the user has
    /// </summary>
    public int MappingFollowerCount { get; init; }

    /// <summary>
    /// Count of ranked beatmaps the user has submitted
    /// </summary>
    public int RankedBeatmapsCount { get; init; }

    /// <summary>
    /// Count of ranked or approved beatmaps the user has submitted
    /// </summary>
    public int RankedAndApprovedBeatmapsCount { get; init; }

    /// <summary>
    /// Count of guest beatmaps the user has submitted
    /// </summary>
    public int GuestBeatmapsCount { get; init; }

    /// <summary>
    /// Count of loved beatmaps the user has submitted
    /// </summary>
    public int LovedBeatmapsCount { get; init; }

    /// <summary>
    /// Count of pending beatmaps the user has submitted
    /// </summary>
    public int PendingBeatmapsCount { get; init; }

    /// <summary>
    /// Count of graveyard beatmaps the user has submitted
    /// </summary>
    public int GraveyardBeatmapsCount { get; init; }

    /// <summary>
    /// Count of unranked the user has submitted
    /// </summary>
    public int UnrankedBeatmapsCount { get; init; }

    /// <summary>
    /// Information about the user's Kudosu
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "CommentTypo")]
    public Kudosu? Kudosu { get; init; }

    /// <summary>
    /// Number of comments the user has made on the osu! website
    /// </summary>
    public int CommentsCount { get; init; }

    /// <summary>
    /// Number of posts the user has made on the osu! forums
    /// </summary>
    public int PostCount { get; init; }

    /// <summary>
    /// User's active tournament banner
    /// </summary>
    /// <remarks>Deprecated in favor of <see cref="ActiveTournamentBanners"/></remarks>
    public ProfileBanner? ActiveTournamentBanner { get; init; }

    /// <summary>
    /// A collection of the user's active tournament banners
    /// </summary>
    public ProfileBanner[] ActiveTournamentBanners { get; init; } = [];

    /// <summary>
    /// A collection of the user's badges
    /// </summary>
    public UserBadge[] Badges { get; init; } = [];

    /// <summary>
    /// A collection of groups the user belongs to
    /// </summary>
    public UserGroup[] Groups { get; init; } = [];

    /// <summary>
    /// A collection of the user's beatmap play counts by month
    /// </summary>
    public MonthlyCounts[] MonthlyPlayCounts { get; init; } = [];

    /// <summary>
    /// A collection of the user's replay watch counts by month
    /// </summary>
    public MonthlyCounts[] ReplaysWatchedCounts { get; init; } = [];

    /// <summary>
    /// A collection of information about the user's achievements
    /// </summary>
    public UserAchievement[] Achievements { get; init; } = [];

    /// <summary>
    /// Information about the user's highest recorded global rank
    /// </summary>
    public HighestRank? HighestRank { get; init; }

    /// <summary>
    /// Information about the user's rank by day for the last 90 days
    /// </summary>
    public RankHistory? RankHistory { get; init; }

    /// <summary>
    /// A collection of information about disciplinary actions taken against the user
    /// </summary>
    public UserAccountHistory[] AccountHistory { get; init; } = [];
}
