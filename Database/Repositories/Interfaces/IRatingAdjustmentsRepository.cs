using Database.Entities;

namespace Database.Repositories.Interfaces;

public interface IRatingAdjustmentsRepository : IRepository<RatingAdjustment>
{
    Task TruncateAsync();
}
