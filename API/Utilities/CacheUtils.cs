using API.Entities;

namespace API.Utilities;

/// <summary>
/// Utility for interfacing with the redis cache
/// </summary>
public static class CacheUtils
{
    public const string TournamentSearchTag = $"{SearchPrefix}_{nameof(Tournament)}";

    public const string MatchSearchTag = $"{SearchPrefix}_{nameof(Match)}";

    public const string PlayerSearchTag = $"{SearchPrefix}_{nameof(Player)}";

    private const string SearchPrefix = "search";

    public static string TournamentSearchKey(string query) =>
        $"{TournamentSearchTag}:{query}";

    public static string MatchSearchKey(string query) =>
        $"{MatchSearchTag}:{query}";

    public static string PlayerSearchKey(string query) =>
        $"{PlayerSearchTag}:{query}";
}
