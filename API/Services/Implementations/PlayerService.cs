using API.DTOs;
using API.Services.Interfaces;
using AutoMapper;
using Database.Entities;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class PlayerService(IPlayersRepository playerRepository, IMapper mapper) : IPlayerService
{
    public async Task<IEnumerable<PlayerCompactDTO>> GetAllAsync() =>
        mapper.Map<IEnumerable<PlayerCompactDTO>>(await playerRepository.GetAsync());

    public async Task<int?> GetIdAsync(int userId)
    {
        return await playerRepository.GetIdAsync(userId);
    }

    public async Task<PlayerCompactDTO?> GetVersatileAsync(string key) =>
        mapper.Map<PlayerCompactDTO?>(await playerRepository.GetVersatileAsync(key, false));

    public async Task<IEnumerable<PlayerCompactDTO>> GetAsync(IEnumerable<long> osuIds)
    {
        var idList = osuIds.ToList();
        var players = (await playerRepository.GetAsync(idList)).ToList();
        var dtos = new List<PlayerCompactDTO>();

        // Iterate through the players, on null items create a default DTO but store the osuId.
        // This tells the caller that we don't have info on a specific player.

        for (var i = 0; i < players.Count; i++)
        {
            Player? curPlayer = players[i];
            if (curPlayer is not null)
            {
                dtos.Add(mapper.Map<PlayerCompactDTO>(curPlayer));
            }
            else
            {
                dtos.Add(new PlayerCompactDTO
                {
                    OsuId = idList.ElementAt(i)
                });
            }
        }

        return dtos;
    }
}
