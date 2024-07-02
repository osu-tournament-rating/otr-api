using OsuApiClient.Enums;

namespace DataWorkerService.Configurations;

/// <summary>
/// Configures the way <see cref="Database.Entities.Player"/> data is fetched from outside sources
/// </summary>
public class PlayerFetchConfiguration
{
    public const string Position = "Players";

    /// <summary>
    /// The <see cref="PlayerFetchPlatformConfiguration"/> for <see cref="FetchPlatform.Osu"/>
    /// </summary>
    public PlayerFetchPlatformConfiguration Osu { get; init; } = new(FetchPlatform.Osu);

    /// <summary>
    /// The <see cref="PlayerFetchPlatformConfiguration"/> <see cref="FetchPlatform.OsuTrack"/>
    /// </summary>
    public PlayerFetchPlatformConfiguration OsuTrack { get; init; } = new(FetchPlatform.OsuTrack);
}

/// <summary>
/// Configures the way <see cref="Database.Entities.Player"/> data is fetched for a single <see cref="FetchPlatform"/>
/// </summary>
/// <param name="platform">
/// The <see cref="FetchPlatform"/> being configured by the <see cref="PlayerFetchPlatformConfiguration"/>
/// </param>
public class PlayerFetchPlatformConfiguration(FetchPlatform platform)
{
    /// <summary>
    /// Denotes if the worker is enabled
    /// </summary>
    public bool Enabled { get; init; } = true;

    /// <summary>
    /// Denotes if the worker should mark all players as outdated on start
    /// </summary>
    /// <remarks>Effectively queues all players to be processed</remarks>
    public bool MarkAllOutdated { get; init; }

    /// <summary>
    /// The number of players to process at once
    /// </summary>
    public int BatchSize { get; init; } = platform switch
    {
        FetchPlatform.Osu => 30,
        FetchPlatform.OsuTrack => 10,
        _ => 5
    };

    /// <summary>
    /// Time to wait between batches in seconds
    /// </summary>
    public int BatchIntervalSeconds { get; init; } = platform switch
    {
        FetchPlatform.Osu => 120,
        FetchPlatform.OsuTrack => 300,
        _ => 120
    };

    /// <summary>
    /// Time before a player's data is considered outdated in days
    /// </summary>
    public int PlayerOutdatedAfterDays { get; init; } = platform switch
    {
        FetchPlatform.Osu => 7,
        FetchPlatform.OsuTrack => 14,
        _ => 7
    };
}
