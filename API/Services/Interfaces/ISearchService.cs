using API.DTOs;

namespace API.Services.Interfaces;

public interface ISearchService
{
    Task<List<SearchResponseDTO>?> SearchByNameAsync(string? tournamentName, string? matchName, string? username);
}
