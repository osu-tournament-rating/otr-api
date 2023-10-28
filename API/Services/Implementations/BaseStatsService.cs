using API.DTOs;
using API.Entities;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;

namespace API.Services.Implementations;

public class BaseStatsService : IBaseStatsService
{
	private readonly IBaseStatsRepository _repository;
	private readonly IMapper _mapper;
	public BaseStatsService(IBaseStatsRepository repository, IMapper mapper)
	{
		_repository = repository;
		_mapper = mapper;
	}
	
	public async Task<IEnumerable<BaseStatsDTO>> GetForPlayerAsync(long osuPlayerId) => _mapper.Map<IEnumerable<BaseStatsDTO>>(await _repository.GetForPlayerAsync(osuPlayerId));
	public async Task<BaseStatsDTO?> GetForPlayerAsync(int id, int mode) => _mapper.Map<BaseStatsDTO?>(await _repository.GetForPlayerAsync(id, mode));

	public async Task<int> BatchInsertAsync(IEnumerable<BaseStatsPostDTO> stats)
	{
		var toInsert = new List<BaseStats>();
		foreach (var item in stats)
		{
			toInsert.Add(new BaseStats
			{
				PlayerId = item.PlayerId,
				Rating = item.Rating,
				Volatility = item.Volatility,
				Mode = item.Mode,
				Percentile = item.Percentile,
				GlobalRank = item.GlobalRank,
				CountryRank = item.CountryRank,
			});
		}
		return await _repository.BatchInsertAsync(toInsert);
	}
	public async Task<IEnumerable<BaseStatsDTO>> GetAllAsync() => _mapper.Map<IEnumerable<BaseStatsDTO>>(await _repository.GetAllAsync());
	public async Task TruncateAsync() => await _repository.TruncateAsync();
	public async Task<DateTime> GetRecentCreatedDate(long osuPlayerId) => await _repository.GetRecentCreatedDate(osuPlayerId);
	public async Task<int?> InsertOrUpdateAsync(int playerId, BaseStats baseStats) => await _repository.InsertOrUpdateForPlayerAsync(playerId, baseStats); 
}