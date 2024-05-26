using System.Diagnostics.CodeAnalysis;
using API.Repositories.Interfaces;
using Database;
using Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class RatingAdjustmentsRepository(OtrContext context) : RepositoryBase<RatingAdjustment>(context), IRatingAdjustmentsRepository
{
    private readonly OtrContext _context = context;

    public async Task TruncateAsync() =>
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE rating_adjustments RESTART IDENTITY");
}
