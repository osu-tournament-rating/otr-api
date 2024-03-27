using API.DTOs;

namespace API.Services.Interfaces;

public interface IScreeningService
{
    /// <summary>
    /// Processes a <see cref="ScreeningDTO"/>, providing a
    /// <see cref="ScreeningResultDTO"/> for each osu! player id provided within
    /// </summary>
    /// <param name="screeningRequest">A screening request to process</param>
    /// <returns></returns>
    Task<IEnumerable<ScreeningResultDTO>> ScreenAsync(ScreeningDTO screeningRequest);
}
