using API.DTOs;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;
using Database.Entities;

namespace API.Services.Implementations;

public class PlayerService(IPlayerRepository playerRepository, IMapper mapper) : IPlayerService
{
    public async Task<IEnumerable<PlayerDTO>> GetAllAsync() =>
        mapper.Map<IEnumerable<PlayerDTO>>(await playerRepository.GetAsync());

    public async Task<IEnumerable<PlayerRanksDTO>> GetAllRanksAsync() =>
        mapper.Map<IEnumerable<PlayerRanksDTO>>(await playerRepository.GetAllAsync());

    public async Task<int?> GetIdAsync(int userId)
    {
        return await playerRepository.GetIdAsync(userId);
    }

    public async Task<IEnumerable<PlayerIdMappingDTO>> GetIdMappingAsync() =>
        await playerRepository.GetIdMappingAsync();

    public async Task<IEnumerable<PlayerCountryMappingDTO>> GetCountryMappingAsync() =>
        await playerRepository.GetCountryMappingAsync();

    public async Task<PlayerInfoDTO?> GetVersatileAsync(string key) =>
        mapper.Map<PlayerInfoDTO?>(await playerRepository.GetVersatileAsync(key, false));

    public async Task<IEnumerable<PlayerInfoDTO>> GetAsync(IEnumerable<long> osuIds)
    {
        var idList = osuIds.ToList();
        var players = (await playerRepository.GetAsync(idList)).ToList();
        var dtos = new List<PlayerInfoDTO>();

        // Iterate through the players, on null items create a default DTO but store the osuId.
        // This tells the caller that we don't have info on a specific player.

        for (var i = 0; i < players.Count; i++)
        {
            Player? curPlayer = players[i];
            if (curPlayer is not null)
            {
                dtos.Add(mapper.Map<PlayerInfoDTO>(curPlayer));
            }
            else
            {
                dtos.Add(new PlayerInfoDTO
                {
                    OsuId = idList.ElementAt(i)
                });
            }
        }

        return dtos;
    }
}
