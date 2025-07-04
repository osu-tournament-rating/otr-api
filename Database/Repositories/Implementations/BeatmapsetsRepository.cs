using Database.Entities;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

public class BeatmapsetsRepository(OtrContext context) : Repository<Beatmapset>(context), IBeatmapsetsRepository
{
    private readonly OtrContext _context = context;

    public async Task<Beatmapset?> GetWithDetailsAsync(long osuId) =>
        LocalView.FirstOrDefault(bs => bs.OsuId == osuId)
        ?? await _context.Beatmapsets
            .Include(bs => bs.Creator)
            .Include(bs => bs.Beatmaps)
            .FirstOrDefaultAsync(bs => bs.OsuId == osuId);

    public async Task<int> MarkBeatmapsAsNoDataAsync(int beatmapsetId)
    {
        return await _context.Beatmaps
            .Where(b => b.BeatmapsetId == beatmapsetId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(b => b.HasData, false));
    }
}
