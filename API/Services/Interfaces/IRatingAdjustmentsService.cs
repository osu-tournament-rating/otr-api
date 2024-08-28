using API.DTOs;

namespace API.Services.Interfaces;

public interface IRatingAdjustmentsService
{
    Task BatchInsertAsync(IEnumerable<RatingAdjustmentDTO> postBody);
    Task TruncateAsync();
}
