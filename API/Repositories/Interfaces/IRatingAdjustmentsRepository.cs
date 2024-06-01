using Database.Entities;
using Database.Repositories.Interfaces;

namespace API.Repositories.Interfaces;

public interface IRatingAdjustmentsRepository : IRepository<RatingAdjustment>
{
    Task TruncateAsync();
}
