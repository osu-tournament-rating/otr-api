namespace DataWorkerService.Configurations;

public class PlayerDataWorkerConfiguration
{
    public const string Position = "Players";

    public PlayerPlatformConfiguration Osu { get; init; } = new(PlayerFetchPlatform.Osu);

    public PlayerPlatformConfiguration OsuTrack { get; init; } = new(PlayerFetchPlatform.OsuTrack);
}

/// <summary>
/// Values that control the way player data is fetched from outside sources
/// </summary>
/// <param name="platform">Loads a default configuration for the given platform</param>
public class PlayerPlatformConfiguration(PlayerFetchPlatform platform)
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
        PlayerFetchPlatform.Osu => 30,
        PlayerFetchPlatform.OsuTrack => 10,
        _ => 5
    };

    /// <summary>
    /// Time to wait between batches in seconds
    /// </summary>
    public int BatchIntervalSeconds { get; init; } = platform switch
    {
        PlayerFetchPlatform.Osu => 120,
        PlayerFetchPlatform.OsuTrack => 300,
        _ => 120
    };

    /// <summary>
    /// Time between fetches for individual players in days
    /// </summary>
    public int PlayerOutdatedAfterDays { get; init; } = platform switch
    {
        PlayerFetchPlatform.Osu => 7,
        PlayerFetchPlatform.OsuTrack => 14,
        _ => 7
    };
}

/// <summary>
/// Outside source used to fetch player data
/// </summary>
public enum PlayerFetchPlatform
{
    Osu,
    OsuTrack
}
