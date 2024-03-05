using API.DTOs;

namespace API.Repositories.Interfaces;

public interface IRatingAdjustmentsRepository
{
    Task BatchInsertAsync(IEnumerable<RatingAdjustmentDTO> postBody);
    Task TruncateAsync();
}
