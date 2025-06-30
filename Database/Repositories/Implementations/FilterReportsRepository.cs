using Database.Entities;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

/// <summary>
/// Repository implementation for FilterReport entities
/// </summary>
public class FilterReportsRepository(OtrContext context)
    : Repository<FilterReport>(context), IFilterReportsRepository
{
    private readonly OtrContext _context = context;

    public async Task<FilterReport?> GetWithPlayersAsync(int id)
    {
        return await _context.FilterReports
            .Include(f => f.FilterReportPlayers)
                .ThenInclude(frp => frp.Player)
            .FirstOrDefaultAsync(f => f.Id == id);
    }
}
