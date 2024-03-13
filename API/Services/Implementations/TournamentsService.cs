using API.DTOs;
using API.Entities;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;

namespace API.Services.Implementations;

public class TournamentsService(ITournamentsRepository tournamentsRepository, IMapper mapper) : ITournamentsService
{
    private readonly ITournamentsRepository _tournamentsRepository = tournamentsRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<TournamentDTO> CreateAsync(TournamentWebSubmissionDTO wrapper)
    {
        Tournament tournament = await _tournamentsRepository.CreateAsync(new Tournament()
        {
            Name = wrapper.TournamentName,
            Abbreviation = wrapper.Abbreviation,
            ForumUrl = wrapper.ForumPost,
            RankRangeLowerBound = wrapper.RankRangeLowerBound,
            Mode = wrapper.Mode,
            TeamSize = wrapper.TeamSize,
            SubmitterUserId = wrapper.SubmitterId
        });
        return _mapper.Map<TournamentDTO>(tournament);
    }

    public async Task<bool> ExistsAsync(int id) => await _tournamentsRepository.ExistsAsync(id);

    public async Task<bool> ExistsAsync(string name, int mode) => await _tournamentsRepository.ExistsAsync(name, mode);

    public async Task<IEnumerable<TournamentDTO>> GetAllAsync()
    {
        IEnumerable<Tournament> items = await _tournamentsRepository.GetAllAsync();
        items = items.OrderBy(x => x.Name);

        return _mapper.Map<IEnumerable<TournamentDTO>>(items);
    }

    public async Task<TournamentDTO?> GetAsync(int id) =>
        _mapper.Map<TournamentDTO>(await _tournamentsRepository.GetAsync(id));

    public async Task<int> CountPlayedAsync(
        int playerId,
        int mode,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    ) => await _tournamentsRepository.CountPlayedAsync(playerId, mode, dateMin, dateMax);

    public async Task<TournamentDTO> UpdateAsync(int id, TournamentDTO wrapper)
    {
        Tournament? existing = await _tournamentsRepository.GetAsync(id) ?? throw new Exception($"Tournament with id {id} does not exist");

        existing.Name = wrapper.Name;
        existing.Abbreviation = wrapper.Abbreviation;
        existing.ForumUrl = wrapper.ForumUrl;
        existing.Mode = wrapper.Mode;
        existing.RankRangeLowerBound = wrapper.RankRangeLowerBound;
        existing.TeamSize = wrapper.TeamSize;
        existing.SubmitterUserId = wrapper.SubmitterUserId;

        await _tournamentsRepository.UpdateAsync(existing);
        return _mapper.Map<TournamentDTO>(existing);
    }
}
