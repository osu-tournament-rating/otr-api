using JetBrains.Annotations;

namespace API.DTOs;

/// <summary>
/// Represents a collection of search results
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class SearchResponseCollectionDTO
{
    /// <summary>
    /// A collection of search results for tournaments matching the search query
    /// </summary>
    public ICollection<TournamentSearchResultDTO> Tournaments { get; set; } = null!;

    /// <summary>
    /// A collection of search results for matches matching the search query
    /// </summary>
    public ICollection<MatchSearchResultDTO> Matches { get; set; } = null!;

    /// <summary>
    /// A collection of search results for players matching the search query
    /// </summary>
    public ICollection<PlayerSearchResultDTO> Players { get; set; } = null!;
}
