using API.DTOs;
using API.Entities;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;

namespace API.Services.Implementations;

public class TournamentsService(ITournamentsRepository repository, IMapper mapper) : ITournamentsService
{
    private readonly ITournamentsRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public async Task<TournamentDTO> CreateOrUpdateAsync(
        TournamentWebSubmissionDTO wrapper,
        bool updateExisting = false
    )
    {
        Entities.Tournament tournament = await _repository.CreateOrUpdateAsync(wrapper, updateExisting);

        return _mapper.Map<TournamentDTO>(tournament);
    }

    public async Task<bool> ExistsAsync(string name, int mode) => await _repository.ExistsAsync(name, mode);

    public async Task<IEnumerable<TournamentDTO>> GetAllAsync()
    {
        IEnumerable<Tournament> items = await _repository.GetAllAsync();
        items = items.OrderBy(x => x.Name);

        return _mapper.Map<IEnumerable<TournamentDTO>>(items);
    }

    public async Task<int> CountPlayedAsync(
        int playerId,
        int mode,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    ) => await _repository.CountPlayedAsync(playerId, mode, dateMin, dateMax);

    public async Task<TournamentDTO?> GetAsync(int id)
    {
        Tournament? tournament = await _repository.GetAsync(id);

        if (tournament == null)
        {
            return null;
        }

        return _mapper.Map<TournamentDTO>(tournament);
    }
}
