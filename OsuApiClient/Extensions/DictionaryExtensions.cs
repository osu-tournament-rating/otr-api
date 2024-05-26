namespace OsuApiClient.Extensions;

internal static class DictionaryExtensions
{
    /// <summary>
    /// Formats a dictionary into a query string
    /// </summary>
    public static string ToQueryString<TKey, TVal>(this IDictionary<TKey, TVal> dictionary) =>
        $"?{string.Join("&", dictionary.Select(x => $"{x.Key}={x.Value}"))}";
}
