using API.DTOs;

namespace API.Services.Interfaces;

public interface IFilteringService
{
    /// <summary>
    /// Processes a <see cref="FilteringRequestDTO"/>, returning a
    /// <see cref="FilteringResultDTO"/> containing a
    /// <see cref="PlayerFilteringResultDTO"/> for each osu! player id provided
    /// </summary>
    /// <param name="request">A filtering request to process</param>
    /// <param name="userId">The ID of the user making the request</param>
    /// <returns>A <see cref="FilteringResultDTO"/> with the results</returns>
    Task<FilteringResultDTO> FilterAsync(FilteringRequestDTO request, int userId);
}
