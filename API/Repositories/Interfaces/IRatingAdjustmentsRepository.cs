using Database.Entities;

namespace API.Repositories.Interfaces;

public interface IRatingAdjustmentsRepository : IRepository<RatingAdjustment>
{
    Task TruncateAsync();
}
