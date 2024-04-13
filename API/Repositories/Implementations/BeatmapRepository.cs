using System.Diagnostics.CodeAnalysis;
using API.Entities;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class BeatmapRepository(OtrContext context) : RepositoryBase<Beatmap>(context), IBeatmapRepository
{
    private readonly OtrContext _context = context;

    public override async Task<IEnumerable<Beatmap>> GetAllAsync() => await _context.Beatmaps.ToListAsync();

    public async Task<Beatmap?> GetAsync(long beatmapId) =>
        await _context.Beatmaps.FirstOrDefaultAsync(x => x.BeatmapId == beatmapId);

    public async Task<int?> GetIdAsync(long beatmapId) => (await GetAsync(beatmapId))?.Id;
}
