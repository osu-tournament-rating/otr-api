using System.ComponentModel.DataAnnotations;

namespace DWS.Configurations;

/// <summary>
/// Configuration for the player osu!track data update background service.
/// </summary>
public class PlayerOsuTrackUpdateServiceConfiguration
{
    public const string Position = "PlayerOsuTrackUpdateService";

    /// <summary>
    /// Whether the background service is enabled.
    /// </summary>
    public bool Enabled { get; init; }

    /// <summary>
    /// Number of days after which player osu!track data is considered outdated.
    /// </summary>
    [Range(1, 365)]
    public int OutdatedAfterDays { get; init; } = 60;

    /// <summary>
    /// Delay in seconds between checking for outdated players.
    /// </summary>
    [Range(60, 86400)]
    public int CheckIntervalSeconds { get; init; } = 300;

    /// <summary>
    /// Maximum number of messages to enqueue per check cycle.
    /// </summary>
    [Range(1, 500)]
    public int MaxMessagesPerCycle { get; init; } = 500;

    /// <summary>
    /// Priority level for player osu!track fetch messages (0 = low, 5 = normal, 10 = high).
    /// Background updates use low priority to avoid interfering with real-time requests.
    /// </summary>
    [Range(0, 10)]
    public byte MessagePriority { get; init; } = 0; // Low priority for background updates
}
