using System.ComponentModel.DataAnnotations;

namespace DWS.Configurations;

/// <summary>
/// Configuration for consumer concurrency limits per queue.
/// Controls the ConcurrentMessageLimit for each endpoint to manage processing throughput.
/// </summary>
public class ConsumerConcurrencyConfiguration
{
    public const string Position = "ConsumerConcurrency";

    /// <summary>
    /// Number of concurrent consumers for beatmap fetch operations.
    /// </summary>
    [Range(1, 10)]
    public int BeatmapFetchConsumers { get; set; } = 1;

    /// <summary>
    /// Number of concurrent consumers for match fetch operations.
    /// </summary>
    [Range(1, 10)]
    public int MatchFetchConsumers { get; set; } = 1;

    /// <summary>
    /// Number of concurrent consumers for player fetch operations.
    /// </summary>
    [Range(1, 10)]
    public int PlayerFetchConsumers { get; set; } = 1;

    /// <summary>
    /// Number of concurrent consumers for player osu!track fetch operations.
    /// </summary>
    [Range(1, 10)]
    public int PlayerOsuTrackFetchConsumers { get; set; } = 1;

    /// <summary>
    /// Number of concurrent consumers for tournament automation check operations.
    /// </summary>
    [Range(1, 10)]
    public int TournamentAutomationCheckConsumers { get; set; } = 1;

    /// <summary>
    /// Number of concurrent consumers for tournament stats processing.
    /// </summary>
    [Range(1, 10)]
    public int TournamentStatsConsumers { get; set; } = 10;
}
