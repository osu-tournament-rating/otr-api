using System.Diagnostics.CodeAnalysis;
using Database.Entities;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class BeatmapsRepository(OtrContext context) : RepositoryBase<Beatmap>(context), IBeatmapsRepository
{
    private readonly OtrContext _context = context;

    public async Task<Beatmap?> GetAsync(long osuId) =>
        LocalView.FirstOrDefault(b => b.OsuId == osuId)
        ?? await _context.Beatmaps.FirstOrDefaultAsync(x => x.OsuId == osuId);
}
