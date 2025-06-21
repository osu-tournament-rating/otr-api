using System.Diagnostics.CodeAnalysis;
using Database.Entities;
using Database.Repositories.Interfaces;
using Database.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

[SuppressMessage("Performance",
    "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class BeatmapsRepository(OtrContext context) : Repository<Beatmap>(context), IBeatmapsRepository
{
    private readonly OtrContext _context = context;

    public async Task<Beatmap?> GetAsync(long osuId) =>
        LocalView.FirstOrDefault(b => b.OsuId == osuId)
        ?? await _context.Beatmaps
            .IncludeChildren()
            .FirstOrDefaultAsync(x => x.OsuId == osuId);

    public async Task<IEnumerable<Beatmap>> GetAsync(IEnumerable<long> osuIds)
    {
        var osuIdsList = osuIds.ToList();

        // Load local instances
        var result = LocalView.Where(b => osuIdsList.Contains(b.OsuId)).ToList();

        // Query db for non-local instances
        var remainingIds = osuIdsList.Except(result.Select(p => p.OsuId)).ToList();
        return (await _context.Beatmaps
            .IncludeChildren()
            .Where(b => remainingIds.Contains(b.OsuId)).ToListAsync()).Concat(result);
    }

    public async Task<ICollection<Beatmap>> GetOrCreateAsync(IEnumerable<long> osuIds, bool save)
    {
        var osuIdsList = osuIds.ToList();

        // Identify existing beatmaps
        var existing = (await GetAsync(osuIdsList)).ToList();

        // Identify missing beatmaps
        var missingIds = osuIdsList.Except(existing.Select(b => b.OsuId)).ToList();

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
