using API.DTOs;

namespace API.Services.Interfaces;

public interface IScreeningService
{
    /// <summary>
    /// Processes a <see cref="ScreeningRequestDTO"/>, providing a
    /// <see cref="PlayerScreeningResultDTO"/> for each osu! player id provided within
    /// </summary>
    /// <param name="screeningRequest">A screening request to process</param>
    /// <returns></returns>
    Task<IEnumerable<PlayerScreeningResultDTO>> ScreenAsync(ScreeningRequestDTO screeningRequest);
}
