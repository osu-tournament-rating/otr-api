using Newtonsoft.Json;

namespace OsuApiClient.Net.JsonModels.Osu.Beatmaps;

/// <summary>
/// Represents a beatmapset
/// </summary>
/// <copyright>
/// ppy 2024 https://osu.ppy.sh/docs/index.html#beatmapset
/// Last accessed June 2024
/// </copyright>
public class BeatmapsetJsonModel : JsonModelBase
{
    [JsonProperty("artist")]
    public string Artist { get; set; } = string.Empty;

    [JsonProperty("artist_unicode")]
    public string ArtistUnicode { get; set; } = string.Empty;

    [JsonProperty("bpm")]
    public double Bpm { get; set; }

    [JsonProperty("creator")]
    public string? Creator { get; set; }

    [JsonProperty("creator_id")]
    public long? CreatorId { get; set; }

    [JsonProperty("favourite_count")]
    public int FavouriteCount { get; set; }

    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("last_updated")]
    public DateTime LastUpdated { get; set; }

    [JsonProperty("nsfw")]
    public bool Nsfw { get; set; }

    [JsonProperty("offset")]
    public double Offset { get; set; }

    [JsonProperty("play_count")]
    public long PlayCount { get; set; }

    /// <summary>
    /// Audio preview
    /// </summary>
    [JsonProperty("preview_url")]
    public string? PreviewUrl { get; set; }

    [JsonProperty("ranked_date")]
    public DateTime? RankedDate { get; set; }

    [JsonProperty("ranked")]
    public int RankedStatus { get; set; }

    [JsonProperty("source")]
    public string? Source { get; set; }

    [JsonProperty("spotlight")]
    public bool Spotlight { get; set; }

    [JsonProperty("submitted_date")]
    public DateTime SubmittedDate { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("title_unicode")]
    public string TitleUnicode { get; set; } = string.Empty;

    [JsonProperty("track_id")]
    public long? TrackId { get; set; }

    [JsonProperty("user_id")]
    public long? UserId { get; set; }

    [JsonProperty("video")]
    public bool Video { get; set; }
}
