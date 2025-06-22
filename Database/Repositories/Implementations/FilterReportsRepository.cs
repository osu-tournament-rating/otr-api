using Database.Entities;
using Database.Repositories.Interfaces;

namespace Database.Repositories.Implementations;

/// <summary>
/// Repository implementation for FilterReport entities
/// </summary>
public class FilterReportsRepository(OtrContext context)
    : Repository<FilterReport>(context), IFilterReportsRepository
{
}
