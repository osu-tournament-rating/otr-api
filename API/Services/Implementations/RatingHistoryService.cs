using API.DTOs;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;

namespace API.Services.Implementations;

public class RatingHistoryService : IRatingHistoryService
{
	private readonly IRatingHistoryRepository _ratingHistoryRepository;
	private readonly IMapper _mapper;
	public RatingHistoryService(IRatingHistoryRepository ratingHistoryRepository, IMapper mapper)
	{
		_ratingHistoryRepository = ratingHistoryRepository;
		_mapper = mapper;
	}
	
	public async Task<IEnumerable<RatingHistoryDTO>> GetForPlayerAsync(long osuPlayerId, int mode, DateTime fromTime) => _mapper.Map<IEnumerable<RatingHistoryDTO>>(await _ratingHistoryRepository.GetForPlayerAsync(osuPlayerId, mode, fromTime));
	public async Task<IEnumerable<RatingHistoryDTO>> GetForPlayerAsync(long osuPlayerId) => _mapper.Map<IEnumerable<RatingHistoryDTO>>(await _ratingHistoryRepository.GetForPlayerAsync(osuPlayerId));
	public async Task<int> BatchInsertAsync(IEnumerable<RatingHistoryDTO> histories) => await _ratingHistoryRepository.BatchInsertAsync(histories);
	public async Task TruncateAsync() => await _ratingHistoryRepository.TruncateAsync();
}