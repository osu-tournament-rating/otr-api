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

	public async Task<IEnumerable<PlayerInfoDTO>> GetAllAsync() => _mapper.Map<IEnumerable<PlayerInfoDTO>>(await _playerRepository.GetAllAsync(true));

	public async Task<PlayerInfoDTO?> GetByOsuIdAsync(long osuId, bool eagerLoad = false, OsuEnums.Mode mode = OsuEnums.Mode.Standard, int offsetDays = -1) =>
		_mapper.Map<PlayerInfoDTO?>(await _playerRepository.GetPlayerByOsuIdAsync(osuId, eagerLoad, (int)mode, offsetDays));

	public async Task<IEnumerable<PlayerInfoDTO>> GetByOsuIdsAsync(IEnumerable<long> osuIds) =>
		_mapper.Map<IEnumerable<PlayerInfoDTO>>(await _playerRepository.GetByOsuIdsAsync(osuIds));

	public async Task<IEnumerable<PlayerRanksDTO>> GetAllRanksAsync() { return _mapper.Map<IEnumerable<PlayerRanksDTO>>(await _playerRepository.GetAllAsync(false)); }

	public async Task<IEnumerable<PlayerRatingDTO>> GetTopRatingsAsync(int n, OsuEnums.Mode mode) =>
		_mapper.Map<IEnumerable<PlayerRatingDTO>>(await _playerRepository.GetTopRatingsAsync(n, mode));

	public async Task<string?> GetUsernameAsync(long osuId) => await _playerRepository.GetUsernameAsync(osuId);

	public async Task<int?> GetIdAsync(long osuId)
	{
		int result = await _playerRepository.GetIdByOsuIdAsync(osuId);
		if (result == default)
		{
			return null;
		}

		return result;
	}

	public async Task<int?> GetIdAsync(int userId) { return await _playerRepository.GetIdByUserIdAsync(userId); }

	public async Task<long?> GetOsuIdAsync(int id)
	{
		long result = await _playerRepository.GetOsuIdAsync(id);
		if (result == default)
		{
			return null;
		}

		return result;
	}

	public async Task<IEnumerable<PlayerIdMappingDTO>> GetIdMappingAsync() => await _playerRepository.GetIdMappingAsync();
	public async Task<Dictionary<int, string?>> GetCountryMappingAsync() => await _playerRepository.GetCountryMappingAsync();
	public async Task<PlayerInfoDTO?> GetAsync(int userId) => _mapper.Map<PlayerInfoDTO?>(await _playerRepository.GetAsync(userId));
	public async Task<PlayerInfoDTO?> GetAsync(string username) => _mapper.Map<PlayerInfoDTO?>(await _playerRepository.GetAsync(username));
}