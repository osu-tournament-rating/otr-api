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

	public async Task<IEnumerable<PlayerDTO>> GetAllAsync() => _mapper.Map<IEnumerable<PlayerDTO>>(await _playerRepository.GetAllAsync());

	public async Task<PlayerDTO?> GetByOsuIdAsync(long osuId, bool eagerLoad = false, OsuEnums.Mode mode = OsuEnums.Mode.Standard, int offsetDays = -1) =>
		_mapper.Map<PlayerDTO?>(await _playerRepository.GetPlayerByOsuIdAsync(osuId, eagerLoad, (int)mode, offsetDays));

	public async Task<IEnumerable<PlayerDTO>> GetByOsuIdsAsync(IEnumerable<long> osuIds) => _mapper.Map<IEnumerable<PlayerDTO>>(await _playerRepository.GetByOsuIdsAsync(osuIds));
	public async Task<IEnumerable<PlayerRanksDTO>> GetAllRanksAsync() => _mapper.Map<IEnumerable<PlayerRanksDTO>>(await _playerRepository.GetAllAsync());

	public async Task<IEnumerable<PlayerRatingDTO>> GetTopRatingsAsync(int n, OsuEnums.Mode mode) =>
		_mapper.Map<IEnumerable<PlayerRatingDTO>>(await _playerRepository.GetTopRatingsAsync(n, mode));

	public async Task<int?> GetIdAsync(long osuId)
	{
		int result = await _playerRepository.GetIdByOsuIdAsync(osuId);
		if (result == default)
		{
			return null;
		}

		return result;
	}

	public async Task<long?> GetOsuIdAsync(int id)
	{
		long result = await _playerRepository.GetOsuIdByIdAsync(id);
		if (result == default)
		{
			return null;
		}

		return result;
	}
}