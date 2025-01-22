using System.Diagnostics.CodeAnalysis;
using OsuApiClient.Domain.Osu.Multiplayer;

namespace OsuApiClient.Domain.Osu.Beatmaps;

/// <summary>
/// Represents a beatmapset of a <see cref="Beatmap"/> played in a <see cref="MultiplayerGame"/>
/// </summary>
public class Beatmapset : IModel
{
    /// <summary>
    /// Id of the beatmapset
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Song artist
    /// </summary>
    public string Artist { get; init; } = string.Empty;

    /// <summary>
    /// Song artist including Unicode characters
    /// </summary>
    public string ArtistUnicode { get; init; } = string.Empty;

    /// <summary>
    /// Beats per minute
    /// </summary>
    public double Bpm { get; init; }

    /// <summary>
    /// Id of the <see cref="Creator"/>
    /// </summary>
    public long? CreatorId { get; init; }

    /// <summary>
    /// Username of the creator
    /// </summary>
    public string? Creator { get; init; }

    /// <summary>
    /// Number of favorites
    /// </summary>
    public int FavouriteCount { get; init; }

    /// <summary>
    /// Recent update time
    /// </summary>
    public DateTime LastUpdated { get; init; }

    /// <summary>
    /// Denotes if the set contains nsfw content
    /// </summary>
    public bool Nsfw { get; init; }

    /// <summary>
    /// Included audio offset
    /// </summary>
    public double Offset { get; init; }

    /// <summary>
    /// Play count
    /// </summary>
    public long PlayCount { get; init; }

    /// <summary>
    /// Preview image url
    /// </summary>
    public string? PreviewUrl { get; init; }

    /// <summary>
    /// When the set entered the ranked category
    /// </summary>
    public DateTime? RankedDate { get; init; }

    /// <summary>
    /// No description
    /// </summary>
    public string? Source { get; init; }

    /// <summary>
    /// Denotes if the set is part of a beatmap spotlight
    /// </summary>
    public bool Spotlight { get; init; }

    /// <summary>
    /// Ranked status
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// When the set was published
    /// </summary>
    public DateTime SubmittedDate { get; init; }

    /// <summary>
    /// Title
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Title including Unicode characters
    /// </summary>
    public string TitleUnicode { get; init; } = string.Empty;

    /// <summary>
    /// Featured Artist track identifier
    /// </summary>
    public long? TrackId { get; init; }

    /// <summary>
    /// Denotes if the set includes video
    /// </summary>
    public bool Video { get; init; }
}
