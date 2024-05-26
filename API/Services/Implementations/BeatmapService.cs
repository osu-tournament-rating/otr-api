using API.DTOs;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;

namespace API.Services.Implementations;

public class BeatmapService(IBeatmapRepository beatmapRepository, IMapper mapper) : IBeatmapService
{
    public async Task<IEnumerable<BeatmapDTO>> ListAsync() =>
        mapper.Map<IEnumerable<BeatmapDTO>>(await beatmapRepository.GetAllAsync());

    public async Task<BeatmapDTO?> GetAsync(int id) =>
        mapper.Map<BeatmapDTO?>(await beatmapRepository.GetAsync(id: id));

    public async Task<BeatmapDTO?> GetAsync(long beatmapId) =>
        mapper.Map<BeatmapDTO?>(await beatmapRepository.GetAsync(beatmapId: beatmapId));

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
