using Database.Entities;

namespace API.Utilities;

/// <summary>
/// Utility for interfacing with the redis cache
/// </summary>
public static class CacheUtils
{
    /// <summary>
    /// Cache tag indicating the object is associated with tournament search results
    /// </summary>
    public const string TournamentSearchTag = $"{SearchPrefix}_{nameof(Tournament)}";

    /// <summary>
    /// Cache tag indicating the object is associated with match search results
    /// </summary>
    public const string MatchSearchTag = $"{SearchPrefix}_{nameof(Match)}";

    /// <summary>
    /// Cache tag indicating the object is associated with player search results
    /// </summary>
    public const string PlayerSearchTag = $"{SearchPrefix}_{nameof(Player)}";

    /// <summary>
    /// String used to prefix all search results
    /// </summary>
    private const string SearchPrefix = "search";

    /// <summary>
    /// Returns the tournament search results cache key for the given query
    /// </summary>
    public static string TournamentSearchKey(string query) =>
        $"{TournamentSearchTag}:{query}";

    /// <summary>
    /// Returns the match search results cache key for the given query
    /// </summary>
    public static string MatchSearchKey(string query) =>
        $"{MatchSearchTag}:{query}";

    /// <summary>
    /// Returns the player search results cache key for the given query
    /// </summary>
    public static string PlayerSearchKey(string query) =>
        $"{PlayerSearchTag}:{query}";
}
