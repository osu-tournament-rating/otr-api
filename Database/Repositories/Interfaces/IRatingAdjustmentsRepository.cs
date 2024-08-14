using Database.Entities.Processor;

namespace Database.Repositories.Interfaces;

public interface IRatingAdjustmentsRepository : IRepository<RatingAdjustment>
{
    Task TruncateAsync();
}
