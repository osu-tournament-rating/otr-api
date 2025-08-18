namespace OsuApiClient.Enums;

/// <summary>
/// Types of resources that can be fetched from external APIs
/// </summary>
public enum FetchResourceType
{
    /// <summary>
    /// Player profile and statistics
    /// </summary>
    Player,

    /// <summary>
    /// Beatmap metadata and difficulty information
    /// </summary>
    Beatmap,

    /// <summary>
    /// Multiplayer match data including games and scores
    /// </summary>
    Match
}
