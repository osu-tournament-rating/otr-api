using API.DTOs;
using API.Entities;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;

namespace API.Services.Implementations;

public class PlayerService(IPlayerRepository playerRepository, IMapper mapper) : IPlayerService
{
    private readonly IMapper _mapper = mapper;
    private readonly IPlayerRepository _playerRepository = playerRepository;

    public async Task<IEnumerable<PlayerDTO>> GetAllAsync() =>
        _mapper.Map<IEnumerable<PlayerDTO>>(await _playerRepository.GetAllAsync());

    public async Task<IEnumerable<PlayerRanksDTO>> GetAllRanksAsync() =>
        _mapper.Map<IEnumerable<PlayerRanksDTO>>(await _playerRepository.GetAllAsync());

    public async Task<PlayerDTO?> GetVersatileAsync(string key) =>
        _mapper.Map<PlayerDTO?>(await _playerRepository.GetVersatileAsync(key));

    public async Task<PlayerDTO?> GetAsync(int id) =>
        _mapper.Map<PlayerDTO?>(await _playerRepository.GetAsync(id));

    public async Task<PlayerDTO?> GetAsync(long osuId) =>
        _mapper.Map<PlayerDTO?>(await _playerRepository.GetAsync(osuId));

    public async Task<PlayerDTO?> GetAsync(string username) =>
        _mapper.Map<PlayerDTO?>(await _playerRepository.GetAsync(username));

    public async Task<PlayerDTO?> UpdateAsync(int id, PlayerDTO wrapper)
    {
        Player? target = await _playerRepository.GetAsync(id);

        if (target is null)
        {
            return null;
        }

        target.OsuId = wrapper.OsuId;
        target.Username = wrapper.Username;
        target.Country = wrapper.Country;

        await _playerRepository.UpdateAsync(target);
        return _mapper.Map<PlayerDTO>(target);
    }
}
