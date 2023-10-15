using API.Controllers;
using API.Entities;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

namespace API.Services.Implementations;

public class TournamentsService : ITournamentsService
{
	private readonly ITournamentsRepository _repository;

	public TournamentsService(ITournamentsRepository repository) { _repository = repository; }
	
	public async Task PopulateAndLinkAsync() => await _repository.PopulateAndLinkAsync();
	public async Task<Tournament> CreateOrUpdateAsync(BatchWrapper wrapper, bool updateExisting = false) => await _repository.CreateOrUpdateAsync(wrapper, updateExisting);
	public async Task<bool> ExistsAsync(string name, int mode) => await _repository.ExistsAsync(name, mode);
}