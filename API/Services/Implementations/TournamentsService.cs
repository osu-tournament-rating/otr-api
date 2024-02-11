using API.DTOs;
using API.Entities;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;

namespace API.Services.Implementations;

public class TournamentsService : ITournamentsService
{
	private readonly ITournamentsRepository _repository;
	private readonly IMapper _mapper;

	public TournamentsService(ITournamentsRepository repository, IMapper mapper)
	{
		_repository = repository;
		_mapper = mapper;
	}

	public async Task<Tournament> CreateOrUpdateAsync(TournamentWebSubmissionDTO wrapper, bool updateExisting = false) => await _repository.CreateOrUpdateAsync(wrapper, updateExisting);
	public async Task<bool> ExistsAsync(string name, int mode) => await _repository.ExistsAsync(name, mode);

	public async Task<IEnumerable<TournamentDTO>> GetAllAsync()
	{
		var items = await _repository.GetAllAsync();
		items = items.OrderBy(x => x.Name);

		return _mapper.Map<IEnumerable<TournamentDTO>>(items);
	}

	public async Task<int> CountPlayedAsync(int playerId, int mode, DateTime? dateMin = null, DateTime? dateMax = null) =>
		await _repository.CountPlayedAsync(playerId, mode, dateMin, dateMax);
}