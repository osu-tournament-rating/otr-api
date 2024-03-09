using API.DTOs;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;

namespace API.Services.Implementations;

public class BeatmapService(IBeatmapRepository beatmapRepository, IMapper mapper) : IBeatmapService
{
    private readonly IBeatmapRepository _beatmapRepository = beatmapRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<BeatmapDTO>> GetAllAsync() =>
        _mapper.Map<IEnumerable<BeatmapDTO>>(await _beatmapRepository.GetAllAsync());

    public async Task<IEnumerable<BeatmapDTO>> GetByBeatmapIdsAsync(IEnumerable<long> beatmapIds) =>
        _mapper.Map<IEnumerable<BeatmapDTO>>(await _beatmapRepository.GetAsync(beatmapIds));

    public async Task<BeatmapDTO?> GetAsync(long osuBeatmapId) =>
        _mapper.Map<BeatmapDTO?>(await _beatmapRepository.GetByOsuIdAsync(osuBeatmapId));
}
