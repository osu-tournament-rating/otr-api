using API.DTOs;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;

namespace API.Services.Implementations;

public class BeatmapService(IBeatmapRepository beatmapRepository, IMapper mapper) : IBeatmapService
{
    private readonly IBeatmapRepository _beatmapRepository = beatmapRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<BeatmapDTO>> ListAsync() =>
        _mapper.Map<IEnumerable<BeatmapDTO>>(await _beatmapRepository.GetAllAsync());

    public async Task<BeatmapDTO?> GetAsync(int id) =>
        _mapper.Map<BeatmapDTO?>(await _beatmapRepository.GetAsync(id: id));

    public async Task<BeatmapDTO?> GetAsync(long beatmapId) =>
        _mapper.Map<BeatmapDTO?>(await _beatmapRepository.GetAsync(beatmapId: beatmapId));

    public async Task<BeatmapDTO?> GetVersatileAsync(string key)
    {
        if (!int.TryParse(key, out var intValue))
        {
            return null;
        }

        // Check for primary key
        BeatmapDTO? result = await GetAsync(intValue);
        if (result is not null)
        {
            return result;
        }

        // Check for beatmapId
        if (long.TryParse(key, out var longValue))
        {
            return await GetAsync(longValue);
        }

        return null;
    }
}
