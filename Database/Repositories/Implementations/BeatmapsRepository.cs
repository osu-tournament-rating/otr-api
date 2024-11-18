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

    public async Task<IEnumerable<Beatmap>> GetAsync(IEnumerable<long> osuIds)
    {
        // Load local instances
        IEnumerable<Beatmap> result = LocalView.Where(b => osuIds.Contains(b.OsuId)).ToList();
        // Query db for non-local instances
        IEnumerable<long> remainingIds = osuIds.Except(result.Select(p => p.OsuId));
        return (await _context.Beatmaps.Where(b => remainingIds.Contains(b.OsuId)).ToListAsync()).Concat(result);
    }

    public async Task<ICollection<Beatmap>> GetOrCreateAsync(IEnumerable<long> osuIds, bool save)
    {
        osuIds = osuIds.ToList();

        // Identify existing beatmaps
        var existing = (await GetAsync(osuIds)).ToList();

        // Identify missing beatmaps
        var missingIds = osuIds.Except(existing.Select(b => b.OsuId)).ToList();

        // Create missing beatmaps
        var missing = missingIds.Select(id => new Beatmap { OsuId = id }).ToList();
        if (!save)
        {
            return existing.Concat(missing).ToList();
        }

        // Save the missing beatmaps
        await _context.Beatmaps.AddRangeAsync(missing);
        await _context.SaveChangesAsync();

        // Return all beatmaps
        return existing.Concat(missing).ToList();
    }
}
