using API.DTOs;
using API.Services.Interfaces;
using AutoMapper;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class PlayerService(IPlayersRepository playerRepository, IMapper mapper) : IPlayerService
{
    public async Task<PlayerCompactDTO?> GetVersatileAsync(string key) =>
        mapper.Map<PlayerCompactDTO?>(await playerRepository.GetVersatileAsync(key, false));

    public async Task<IEnumerable<PlayerCompactDTO>> GetAsync(IEnumerable<long> osuIds)
    {
        return mapper.Map<IEnumerable<PlayerCompactDTO>>(await playerRepository.GetAsync(osuIds));
    }
}
