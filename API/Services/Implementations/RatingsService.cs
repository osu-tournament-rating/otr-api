using API.DTOs;
using API.Entities;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;

namespace API.Services.Implementations;

public class RatingsService : IRatingsService
{
	private readonly IRatingsRepository _repository;
	private readonly IMapper _mapper;
	public RatingsService(IRatingsRepository repository, IMapper mapper)
	{
		_repository = repository;
		_mapper = mapper;
	}
	
	public async Task<IEnumerable<RatingDTO>> GetForPlayerAsync(long osuPlayerId) => _mapper.Map<IEnumerable<RatingDTO>>(await _repository.GetForPlayerAsync(osuPlayerId));
	public async Task<int> BatchInsertAsync(IEnumerable<RatingDTO> ratings) => await _repository.BatchInsertAsync(ratings);
	public async Task<IEnumerable<RatingDTO>> GetAllAsync() => _mapper.Map<IEnumerable<RatingDTO>>(await _repository.GetAllAsync());
	public async Task TruncateAsync() => await _repository.TruncateAsync();
	public async Task<DateTime> GetRecentCreatedDate(long osuPlayerId) => await _repository.GetRecentCreatedDate(osuPlayerId);
	public async Task<int?> InsertOrUpdateAsync(int playerId, Rating rating) => await _repository.InsertOrUpdateForPlayerAsync(playerId, rating); 
}