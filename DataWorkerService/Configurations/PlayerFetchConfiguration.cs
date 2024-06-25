using System.Diagnostics.CodeAnalysis;
using OsuApiClient.Enums;

namespace DataWorkerService.Configurations;

/// <summary>
/// Values that control the way player data is fetched from outside sources
/// </summary>
public class PlayerDataWorkerConfiguration
{
    public const string Position = "Players";

    /// <summary>
    /// Configuration for <see cref="FetchPlatform.Osu"/>
    /// </summary>
    public PlayerPlatformConfiguration Osu { get; init; } = new(FetchPlatform.Osu);

    /// <summary>
    /// Configuration for <see cref="FetchPlatform.OsuTrack"/>
    /// </summary>
    public PlayerPlatformConfiguration OsuTrack { get; init; } = new(FetchPlatform.OsuTrack);
}

/// <summary>
/// Values that control the way player data is fetched for a single <see cref="FetchPlatform"/>
/// </summary>
/// <param name="platform">Loads a default configuration for the given platform</param>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class PlayerPlatformConfiguration(FetchPlatform platform)
{
    /// <summary>
    /// Denotes if the worker should fetch data for players
    /// </summary>
    public bool AllowFetch { get; init; } = true;

    /// <summary>
    /// Denotes if the worker should mark all players as outdated on start
    /// </summary>
    public bool MarkAllOutdated { get; init; }

    /// <summary>
    /// The number of players to fetch in succession
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
    /// Time between fetches for individual players in days
    /// </summary>
    public int PlayerOutdatedAfterDays { get; init; } = platform switch
    {
        FetchPlatform.Osu => 7,
        FetchPlatform.OsuTrack => 14,
        _ => 7
    };
}
