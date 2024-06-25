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
    public string Artist { get; set; } = null!;

    [JsonProperty("artist_unicode")]
    public string ArtistUnicode { get; set; } = null!;

    [JsonProperty("covers")]
    public BeatmapsetCoversJsonModel Covers { get; set; } = null!;

    [JsonProperty("creator")]
    public string? Creator { get; set; }

    [JsonProperty("favourite_count")]
    public int FavouriteCount { get; set; }

    [JsonProperty("hype")]
    public BeatmapsetHypeJsonModel? Hype { get; set; }

    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("nsfw")]
    public bool Nsfw { get; set; }

    [JsonProperty("offset")]
    public double Offset { get; set; }

    [JsonProperty("play_count")]
    public long PlayCount { get; set; }

    [JsonProperty("preview_url")]
    public string? PreviewUrl { get; set; }

    [JsonProperty("source")]
    public string? Source { get; set; }

    [JsonProperty("spotlight")]
    public bool Spotlight { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; } = null!;

    [JsonProperty("title")]
    public string Title { get; set; } = null!;

    [JsonProperty("title_unicode")]
    public string TitleUnicode { get; set; } = null!;

    [JsonProperty("track_id")]
    public long? TrackId { get; set; }

    [JsonProperty("user_id")]
    public int? UserId { get; set; }

    [JsonProperty("video")]
    public bool Video { get; set; }
}
