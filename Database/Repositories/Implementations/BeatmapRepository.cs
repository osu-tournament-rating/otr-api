using System.Diagnostics.CodeAnalysis;
using Database.Entities;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class BeatmapRepository(OtrContext context) : RepositoryBase<Beatmap>(context), IBeatmapRepository
{
    private readonly OtrContext _context = context;

    public async Task<Beatmap?> GetAsync(long beatmapId) =>
        await _context.Beatmaps.FirstOrDefaultAsync(x => x.BeatmapId == beatmapId);

    public async Task<int?> GetIdAsync(long beatmapId) => (await GetAsync(beatmapId))?.Id;
}