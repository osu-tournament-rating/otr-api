using API.DTOs;

namespace API.Services.Interfaces;

public interface IScreeningService
{
    /// <summary>
    /// Processes a <see cref="ScreeningRequestDTO"/>, returning a
    /// <see cref="ScreeningResultDTO"/> containing a
    /// <see cref="PlayerScreeningResultDTO"/> for each osu! player id provided within
    /// </summary>
    /// <param name="screeningRequest">A screening request to process</param>
    /// <returns>A <see cref="ScreeningResultDTO"/> with the results</returns>
    Task<ScreeningResultDTO> ScreenAsync(ScreeningRequestDTO screeningRequest);
}
