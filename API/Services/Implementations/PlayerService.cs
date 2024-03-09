using API.DTOs;
using API.Osu;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;

namespace API.Services.Implementations;

public class PlayerService : IPlayerService
{
    private readonly IMapper _mapper;
    private readonly IPlayerRepository _playerRepository;

    public PlayerService(IPlayerRepository playerRepository, IMapper mapper)
    {
        _playerRepository = playerRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PlayerDTO>> GetAllAsync() =>
        _mapper.Map<IEnumerable<PlayerDTO>>(await _playerRepository.GetAsync());

    public async Task<IEnumerable<PlayerRanksDTO>> GetAllRanksAsync() =>
        _mapper.Map<IEnumerable<PlayerRanksDTO>>(await _playerRepository.GetAllAsync());

    public async Task<IEnumerable<PlayerRatingDTO>> GetTopRatingsAsync(int n, OsuEnums.Mode mode) =>
        _mapper.Map<IEnumerable<PlayerRatingDTO>>(await _playerRepository.GetTopRatingsAsync(n, mode));

    public async Task<string?> GetUsernameAsync(long osuId) =>
        await _playerRepository.GetUsernameAsync(osuId);

    public async Task<int?> GetIdAsync(long osuId)
    {
        return await _playerRepository.GetIdAsync(osuId);
    }

    public async Task<int?> GetIdAsync(int userId)
    {
        return await _playerRepository.GetIdAsync(userId);
    }

    public async Task<long?> GetOsuIdAsync(int id)
    {
        long result = await _playerRepository.GetOsuIdAsync(id);
        if (result == default)
        {
            return null;
        }

        return result;
    }

    public async Task<IEnumerable<PlayerIdMappingDTO>> GetIdMappingAsync() =>
        await _playerRepository.GetIdMappingAsync();

    public async Task<IEnumerable<PlayerCountryMappingDTO>> GetCountryMappingAsync() =>
        await _playerRepository.GetCountryMappingAsync();

    public async Task<PlayerInfoDTO?> GetVersatileAsync(string key)
    {
        if (!int.TryParse(key, out int value))
        {
            return await GetAsync(key);
        }

        // Check for the player id
        var result = await GetAsync(value);

        if (result != null)
        {
            return result;
        }

        // Check for the osu id
        if (long.TryParse(key, out long longValue))
        {
            return await GetAsync(longValue);
        }

        return await GetAsync(key);
    }

    public async Task<PlayerInfoDTO?> GetAsync(int userId) =>
        _mapper.Map<PlayerInfoDTO?>(await _playerRepository.GetAsync(userId));

    public async Task<PlayerInfoDTO?> GetAsync(long osuId)
    {
        int? id = await GetIdAsync(osuId);

        return id == null ? null : _mapper.Map<PlayerInfoDTO?>(await _playerRepository.GetAsync(id.Value));
    }

    public async Task<PlayerInfoDTO?> GetAsync(string username) =>
        _mapper.Map<PlayerInfoDTO?>(await _playerRepository.GetAsync(username));
}
