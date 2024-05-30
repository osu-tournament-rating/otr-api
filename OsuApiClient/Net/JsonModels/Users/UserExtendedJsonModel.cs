using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using OsuApiClient.Net.JsonModels.Users.Attributes;

namespace OsuApiClient.Net.JsonModels.Users;

/// <summary>
/// Represents a user
/// </summary>
/// <remarks>Extends <see cref="UserJsonModel"/> with additional attributes</remarks>
/// <copyright>
/// ppy 2024 https://osu.ppy.sh/docs/index.html#user
/// Last accessed May 2024
/// </copyright>
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class UserExtendedJsonModel : UserJsonModel
{
    [JsonProperty("cover_url")]
    public string CoverUrl { get; set; } = null!;

    [JsonProperty("discord")]
    public string? Discord { get; set; }

    [JsonProperty("has_supported")]
    public bool HasSupported { get; set; }

    [JsonProperty("interests")]
    public string? Interests { get; set; }

    [JsonProperty("join_date")]
    public DateTimeOffset JoinDate { get; set; }

    [JsonProperty("location")]
    public string? Location { get; set; }

    [JsonProperty("max_blocks")]
    public int MaxBlocks { get; set; }

    [JsonProperty("max_friends")]
    public int MaxFriends { get; set; }

    [JsonProperty("occupation")]
    public string? Occupation { get; set; }

    [JsonProperty("playmode")]
    public string? PlayMode { get; set; }

    [JsonProperty("playstyle")]
    public string[] PlayStyle { get; set; } = [];

    [JsonProperty("post_count")]
    public int PostCount { get; set; }

    [JsonProperty("profile_order")]
    public string[] ProfileOrder { get; set; } = [];

    [JsonProperty("title")]
    public string? Title { get; set; }

    [JsonProperty("title_url")]
    public string? TitleUrl { get; set; }

    [JsonProperty("twitter")]
    public string? Twitter { get; set; }

    [JsonProperty("website")]
    public string? Website { get; set; }

    [JsonProperty("country")]
    public CountryJsonModel? Country { get; set; }

    [JsonProperty("cover")]
    public CoverJsonModel? Cover { get; set; }

    [JsonProperty("kudosu")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public KudosuJsonModel? Kudosu { get; set; }

    [JsonProperty("account_history")]
    public UserAccountHistoryJsonModel[] AccountHistory { get; set; } = [];

    [JsonProperty("active_tournament_banner")]
    public ProfileBannerJsonModel? ActiveTournamentBanner { get; set; }

    [JsonProperty("active_tournament_banners")]
    public ProfileBannerJsonModel[] ActiveTournamentBanners { get; set; } = [];

    [JsonProperty("badges")]
    public UserBadgeJsonModel[] Badges { get; set; } = [];

    [JsonProperty("beatmaps_playcounts_counts")]
    public long BeatmapPlayCount { get; set; }

    [JsonProperty("comments_count")]
    public int CommentsCount { get; set; }

    [JsonProperty("favourite_beatmapset_count")]
    public int FavouriteBeatmapsCount { get; set; }

    [JsonProperty("follower_count")]
    public int FollowerCount { get; set; }

    [JsonProperty("graveyard_beatmapset_count")]
    public int GraveyardBeatmapsCount { get; set; }

    [JsonProperty("groups")]
    public UserGroupJsonModel[] Groups { get; set; } = [];

    [JsonProperty("guest_beatmapset_count")]
    public int GuestBeatmapsCount { get; set; }

    [JsonProperty("loved_beatmapset_count")]
    public int LovedBeatmapsCount { get; set; }

    [JsonProperty("mapping_follower_count")]
    public int MappingFollowerCount { get; set; }

    [JsonProperty("monthly_playcounts")]
    public MonthlyCountsJsonModel[] MonthlyPlayCounts { get; set; } = [];

    // Deserializer has a hard time with the raw HTML for some reason
    // [JsonProperty("page")]
    // public string? Page { get; set; }

    [JsonProperty("pending_beatmapset_count")]
    public int PendingBeatmapsCount { get; set; }

    [JsonProperty("previous_usernames")]
    public string[] PreviousUsernames { get; set; } = [];

    [JsonProperty("rank_highest")]
    public HighestRankJsonModel? HighestRank { get; set; }

    [JsonProperty("ranked_beatmapset_count")]
    public int RankedBeatmapsCount { get; set; }

    [JsonProperty("replays_watched_counts")]
    public MonthlyCountsJsonModel[] ReplaysWatchedCounts { get; set; } = [];

    [JsonProperty("scores_best_count")]
    public int BestScoresCount { get; set; }

    [JsonProperty("scores_first_count")]
    public int FirstScoresCount { get; set; }

    [JsonProperty("scores_pinned_count")]
    public int PinnedScoresCount { get; set; }

    [JsonProperty("scores_recent_count")]
    public int RecentScoresCount { get; set; }

    [JsonProperty("user_achievements")]
    public UserAchievementJsonModel[] Achievements { get; set; } = [];

    [JsonProperty("rank_history")]
    public RankHistoryJsonModel? RankHistory { get; set; }

    [JsonProperty("ranked_and_approved_beatmapset_count")]
    public int RankedAndApprovedBeatmapsCount { get; set; }

    [JsonProperty("unranked_beatmapset_count")]
    public int UnrankedBeatmapsCount { get; set; }
}
