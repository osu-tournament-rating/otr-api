using API.DTOs;
using API.Services.Interfaces;
using AutoMapper;
using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class TournamentsService(
    ITournamentsRepository tournamentsRepository,
    IMatchesRepository matchesRepository,
    IBeatmapsRepository beatmapsRepository,
    IMapper mapper
    ) : ITournamentsService
{
    public async Task<TournamentCreatedResultDTO> CreateAsync(
        TournamentSubmissionDTO submission,
        int submitterUserId,
        bool preApprove
    )
    {
        // Filter submitted mp ids for those that don't already exist
        var submittedMatchIds = submission.Ids.Distinct().ToList();
        var existingMatchIds = (await matchesRepository.GetAsync(submittedMatchIds))
            .Select(m => m.OsuId)
            .ToList();

        // Filter submitted beatmap ids for those that don't already exist
        var submittedBeatmapIds = submission.BeatmapIds.Distinct().ToList();
        var existingBeatmaps = (await beatmapsRepository.GetAsync(submittedBeatmapIds)).ToList();

        var newTournament = new Tournament
        {
            Name = submission.Name,
            Abbreviation = submission.Abbreviation,
            ForumUrl = submission.ForumUrl,
            RankRangeLowerBound = submission.RankRangeLowerBound,
            Ruleset = submission.Ruleset,
            LobbySize = submission.LobbySize,
            ProcessingStatus = preApprove
                ? TournamentProcessingStatus.NeedsMatchData
                : TournamentProcessingStatus.NeedsApproval,
            SubmittedByUserId = submitterUserId,
            Matches = submittedMatchIds
                .Except(existingMatchIds)
                .Select(matchId => new Match { OsuId = matchId, SubmittedByUserId = submitterUserId })
                .ToList(),
            PooledBeatmaps = existingBeatmaps
                .Concat(submittedBeatmapIds
                    .Except(existingBeatmaps.Select(b => b.OsuId))
                    .Select(beatmapId => new Beatmap { OsuId = beatmapId })
                )
                .ToList()
        };

        // Handle reject-on-submit cases
        if (submission.RejectionReason.HasValue && submission.RejectionReason.Value is not TournamentRejectionReason.None)
        {
            newTournament.ProcessingStatus = TournamentProcessingStatus.Done;
            newTournament.RejectionReason = submission.RejectionReason.Value;

            newTournament.VerificationStatus = VerificationStatus.Rejected;
            newTournament.VerifiedByUserId = submitterUserId;

            foreach (Match match in newTournament.Matches)
            {
                match.ProcessingStatus = MatchProcessingStatus.Done;
                match.RejectionReason = MatchRejectionReason.RejectedTournament;

                match.VerificationStatus = VerificationStatus.Rejected;
                match.VerifiedByUserId = submitterUserId;
            }
        }

        Tournament tournament = await tournamentsRepository.CreateAsync(newTournament);
        return mapper.Map<TournamentCreatedResultDTO>(tournament);
    }

    public async Task<bool> ExistsAsync(int id) =>
        await tournamentsRepository.ExistsAsync(id);

    public async Task<bool> ExistsAsync(string name, Ruleset ruleset)
        => await tournamentsRepository.ExistsAsync(name, ruleset);

    public async Task<IEnumerable<TournamentDTO>> ListAsync()
    {
        IEnumerable<Tournament> items = await tournamentsRepository.GetAllAsync();
        items = items.OrderBy(x => x.Name);

        return mapper.Map<IEnumerable<TournamentDTO>>(items);
    }

    public async Task<TournamentDTO?> GetAsync(int id, bool eagerLoad = true) =>
        mapper.Map<TournamentDTO?>(await tournamentsRepository.GetAsync(id, eagerLoad));

    public async Task<ICollection<TournamentDTO>> GetAsync(TournamentRequestQueryDTO requestQuery)
    {
        return mapper.Map<ICollection<TournamentDTO>>(await tournamentsRepository.GetAsync(requestQuery.Page, requestQuery.PageSize,
            requestQuery.Verified, requestQuery.Ruleset));
    }

    public async Task<TournamentDTO?> GetVerifiedAsync(int id) =>
        mapper.Map<TournamentDTO?>(await tournamentsRepository.GetVerifiedAsync(id));

    public async Task<int> CountPlayedAsync(
        int playerId,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    ) => await tournamentsRepository.CountPlayedAsync(playerId, ruleset, dateMin ?? DateTime.MinValue,
        dateMax ?? DateTime.MaxValue);

    public async Task<TournamentDTO?> UpdateAsync(int id, TournamentDTO wrapper)
    {
        Tournament? existing = await tournamentsRepository.GetAsync(id);
        if (existing is null)
        {
            return null;
        }

        existing.Name = wrapper.Name;
        existing.Abbreviation = wrapper.Abbreviation;
        existing.ForumUrl = wrapper.ForumUrl;
        existing.Ruleset = wrapper.Ruleset;
        existing.RankRangeLowerBound = wrapper.RankRangeLowerBound;
        existing.LobbySize = wrapper.LobbySize;

        await tournamentsRepository.UpdateAsync(existing);
        return mapper.Map<TournamentDTO>(existing);
    }

    public async Task DeleteAsync(int id) => await tournamentsRepository.DeleteAsync(id);
}
