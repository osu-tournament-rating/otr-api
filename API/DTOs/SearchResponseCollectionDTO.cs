namespace API.DTOs;

/// <summary>
/// Represents a collection of search results
/// </summary>
public class SearchResponseCollectionDTO
{
    /// <summary>
    /// A collection of search results for tournaments matching the search query
    /// </summary>
    public SearchResponseDTO[] Tournaments { get; set; } = null!;

    /// <summary>
    /// A collection of search results for matches matching the search query
    /// </summary>
    public SearchResponseDTO[] Matches { get; set; } = null!;

    /// <summary>
    /// A collection of search results for players matching the search query
    /// </summary>
    public SearchResponseDTO[] Players { get; set; } = null!;
}
