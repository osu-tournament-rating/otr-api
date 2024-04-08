using API.DTOs;

namespace API.Services.Interfaces;

public interface ISearchService
{
    /// <summary>
    /// Gets possible matching tournaments, matches, or usernames for the given search key
    /// </summary>
    /// <remarks>Searching by name is always case-insensitive and partial match enabled</remarks>
    /// <returns>A list of all possible matching results</returns>
    Task<SearchResponseCollectionDTO?> SearchByNameAsync(string searchKey);
}
