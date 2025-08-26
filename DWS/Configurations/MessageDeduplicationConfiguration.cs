using System.ComponentModel.DataAnnotations;

namespace DWS.Configurations;

/// <summary>
/// Configuration for message deduplication service
/// </summary>
public class MessageDeduplicationConfiguration
{
    public const string Position = "MessageDeduplication";

    /// <summary>
    /// Whether message deduplication is enabled globally
    /// </summary>
    public bool Enabled { get; init; } = true;

    /// <summary>
    /// Configuration for player fetch deduplication
    /// </summary>
    [Required]
    public FetchDeduplicationSettings PlayerFetch { get; init; } = new();

    /// <summary>
    /// Configuration for player osu!track fetch deduplication
    /// </summary>
    [Required]
    public FetchDeduplicationSettings PlayerOsuTrackFetch { get; init; } = new();

    /// <summary>
    /// Configuration for beatmap fetch deduplication
    /// </summary>
    [Required]
    public FetchDeduplicationSettings BeatmapFetch { get; init; } = new() { Enabled = false };

    /// <summary>
    /// Configuration for match fetch deduplication
    /// </summary>
    [Required]
    public FetchDeduplicationSettings MatchFetch { get; init; } = new() { Enabled = false };
}

/// <summary>
/// Deduplication settings for a specific fetch type
/// </summary>
public class FetchDeduplicationSettings
{
    /// <summary>
    /// Whether deduplication is enabled for this fetch type
    /// </summary>
    public bool Enabled { get; init; } = true;

    /// <summary>
    /// TTL in seconds for pending reservations
    /// </summary>
    public int PendingTtlSeconds { get; init; } = 3600; // 1 hour default

    /// <summary>
    /// TTL in seconds for recently processed cache
    /// </summary>
    public int ProcessedTtlSeconds { get; init; } = 1800; // 30 minutes default
}
