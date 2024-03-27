using API.DTOs;
using API.Entities;
using API.Enums;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;

namespace API.Services.Implementations;

public class TournamentsService(ITournamentsRepository tournamentsRepository, IMapper mapper) : ITournamentsService
{
    private readonly ITournamentsRepository _tournamentsRepository = tournamentsRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<TournamentCreatedResultDTO> CreateAsync(TournamentWebSubmissionDTO wrapper, bool verify, int? verificationSource)
    {
        MatchVerificationStatus verificationStatus = verify
            ? MatchVerificationStatus.Verified
            : MatchVerificationStatus.PendingVerification;

        Tournament tournament = await _tournamentsRepository.CreateAsync(new Tournament
        {
            Name = wrapper.TournamentName,
            Abbreviation = wrapper.Abbreviation,
            ForumUrl = wrapper.ForumPost,
            RankRangeLowerBound = wrapper.RankRangeLowerBound,
            Mode = wrapper.Mode,
            TeamSize = wrapper.TeamSize,
            SubmitterUserId = wrapper.SubmitterId,
            Matches = wrapper.Ids.Select(matchId => new Match
            {
                MatchId = matchId,
                VerificationStatus = (int)verificationStatus,
                NeedsAutoCheck = true,
                IsApiProcessed = false,
                VerificationSource = verificationSource,
                VerifierUserId = verify ? wrapper.SubmitterId : null,
                SubmitterUserId = wrapper.SubmitterId
            }).ToList()
        });
        return _mapper.Map<TournamentCreatedResultDTO>(tournament);
    }

    public async Task<bool> ExistsAsync(int id) => await _tournamentsRepository.ExistsAsync(id);

    public async Task<bool> ExistsAsync(string name, int mode) => await _tournamentsRepository.ExistsAsync(name, mode);

    public async Task<IEnumerable<TournamentDTO>> ListAsync()
    {
        IEnumerable<Tournament> items = await _tournamentsRepository.GetAllAsync();
        items = items.OrderBy(x => x.Name);

        return _mapper.Map<IEnumerable<TournamentDTO>>(items);
    }

    public async Task<TournamentDTO?> GetAsync(int id, bool eagerLoad = true) =>
        _mapper.Map<TournamentDTO>(await _tournamentsRepository.GetAsync(id, eagerLoad));

    public async Task<int> CountPlayedAsync(
        int playerId,
        int mode,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    ) => await _tournamentsRepository.CountPlayedAsync(playerId, mode, dateMin ?? DateTime.MinValue, dateMax ?? DateTime.MaxValue);

    public async Task<TournamentDTO?> UpdateAsync(int id, TournamentDTO wrapper)
    {
        Tournament? existing = await _tournamentsRepository.GetAsync(id);
        if (existing is null)
        {
            return null;
        }

        existing.Name = wrapper.Name;
        existing.Abbreviation = wrapper.Abbreviation;
        existing.ForumUrl = wrapper.ForumUrl;
        existing.Mode = wrapper.Mode;
        existing.RankRangeLowerBound = wrapper.RankRangeLowerBound;
        existing.TeamSize = wrapper.TeamSize;

        await _tournamentsRepository.UpdateAsync(existing);
        return _mapper.Map<TournamentDTO>(existing);
    }
}
