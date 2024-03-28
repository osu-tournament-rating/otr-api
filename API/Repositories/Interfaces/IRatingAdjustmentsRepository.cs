using API.DTOs;
using API.Entities;

namespace API.Repositories.Interfaces;

public interface IRatingAdjustmentsRepository : IRepository<RatingAdjustment>
{
    Task BatchInsertAsync(IEnumerable<RatingAdjustmentDTO> postBody);
    Task TruncateAsync();
}
