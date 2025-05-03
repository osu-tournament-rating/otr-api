using API.DTOs;
using API.Services.Interfaces;
using AutoMapper;
using Database.Entities;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class PlayerService(IPlayersRepository playerRepository, IMapper mapper) : IPlayerService
{
    public async Task<PlayerCompactDTO?> GetVersatileAsync(string key) =>
        mapper.Map<PlayerCompactDTO?>(await playerRepository.GetVersatileAsync(key, false));

    public async Task<IEnumerable<PlayerCompactDTO?>> GetAsync(IEnumerable<long> osuIds)
    {
        var idList = osuIds.ToList();

        // Get players and create a dictionary with OsuId as key
        var players = (await playerRepository.GetAsync(idList))
            .Where(p => p is not null)
            .ToDictionary(p => p!.OsuId);  // Assuming Player has OsuId property

        // Return a list matching the input order:
        // - If the player exists in our dictionary, return the mapped DTO
        // - If not found, return null
        return idList.Select(id =>
            players.TryGetValue(id, out Player? player)
                ? mapper.Map<PlayerCompactDTO>(player)
                : null);
    }
}
