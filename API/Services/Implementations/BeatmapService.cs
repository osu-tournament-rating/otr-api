using API.DTOs;
using API.Services.Interfaces;
using AutoMapper;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class BeatmapService(IBeatmapsRepository beatmapsRepository, IMapper mapper) : IBeatmapService
{
    public async Task<IEnumerable<BeatmapDTO>> ListAsync() =>
        mapper.Map<IEnumerable<BeatmapDTO>>(await beatmapsRepository.GetAllAsync());

    public async Task<BeatmapDTO?> GetAsync(int id) =>
        mapper.Map<BeatmapDTO?>(await beatmapsRepository.GetAsync(id: id));

    public async Task<BeatmapDTO?> GetAsync(long beatmapId) =>
        mapper.Map<BeatmapDTO?>(await beatmapsRepository.GetAsync(beatmapId: beatmapId));

    public async Task<BeatmapDTO?> GetVersatileAsync(long key)
    {
        // Check for primary key
        BeatmapDTO? result = await GetAsync((int)key);
        if (result is not null)
        {
            return result;
        }
        // Check for beatmap id
        return await GetAsync(key);
    }
}
