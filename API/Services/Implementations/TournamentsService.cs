using API.DTOs;
using API.Services.Interfaces;
using AutoMapper;
using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;
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
            Matches = [.. submittedMatchIds
                .Except(existingMatchIds)
                .Select(matchId => new Match { OsuId = matchId, SubmittedByUserId = submitterUserId })],
            PooledBeatmaps =
            [
                .. existingBeatmaps,
                .. submittedBeatmapIds
                            .Except(existingBeatmaps.Select(b => b.OsuId))
                            .Select(beatmapId => new Beatmap { OsuId = beatmapId })
            ]
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

    public async Task<TournamentDTO?> GetAsync(int id, bool eagerLoad = true) =>
        mapper.Map<TournamentDTO?>(await tournamentsRepository.GetAsync(id, eagerLoad));

    public async Task<ICollection<TournamentDTO>> GetAsync(TournamentRequestQueryDTO requestQuery)
    {
        return mapper.Map<ICollection<TournamentDTO>>(await tournamentsRepository.GetAsync(
            requestQuery.Page,
            requestQuery.PageSize,
            requestQuery.Sort,
            verified: requestQuery.Verified,
            ruleset: requestQuery.Ruleset,
            searchQuery: requestQuery.SearchQuery,
            dateMin: requestQuery.DateMin,
            dateMax: requestQuery.DateMax,
            verificationStatus: requestQuery.VerificationStatus,
            rejectionReason: requestQuery.RejectionReason,
            processingStatus: requestQuery.ProcessingStatus,
            submittedBy: requestQuery.SubmittedBy,
            verifiedBy: requestQuery.VerifiedBy,
            lobbySize: requestQuery.LobbySize,
            descending: requestQuery.Descending
        ));
    }


    public async Task<int> CountPlayedAsync(
        int playerId,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    ) => await tournamentsRepository.CountPlayedAsync(playerId, ruleset, dateMin ?? DateTime.MinValue,
        dateMax ?? DateTime.MaxValue);

    public async Task<Dictionary<int, int>> CountPlayedAsync(
        IEnumerable<int> playerIds,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null
    ) => await tournamentsRepository.CountPlayedAsync(playerIds, ruleset, dateMin ?? DateTime.MinValue,
        dateMax ?? DateTime.MaxValue);

    public async Task<TournamentCompactDTO?> UpdateAsync(int id, TournamentCompactDTO wrapper)
    {
        Tournament? existing = await tournamentsRepository.GetAsync(id);
        if (existing is null)
        {
            return null;
        }

        mapper.Map(wrapper, existing);

        if (wrapper.VerificationStatus == VerificationStatus.Rejected)
        {
            await tournamentsRepository.LoadMatchesWithGamesAndScoresAsync(existing);
            existing.RejectAllChildren();
        }

        await tournamentsRepository.UpdateAsync(existing);
        return mapper.Map<TournamentCompactDTO>(existing);
    }

    public async Task DeleteAsync(int id) => await tournamentsRepository.DeleteAsync(id);

    public async Task<TournamentDTO?> AcceptPreVerificationStatusesAsync(int id, int verifierUserId) =>
        mapper.Map<TournamentDTO?>(await tournamentsRepository.AcceptPreVerificationStatusesAsync(id, verifierUserId));

    public async Task RerunAutomationChecksAsync(int id, bool force = false) =>
        await tournamentsRepository.ResetAutomationStatusesAsync(id, force);

    public async Task<ICollection<BeatmapDTO>> AddPooledBeatmapsAsync(int id, ICollection<long> osuBeatmapIds) =>
        mapper.Map<ICollection<BeatmapDTO>>(await tournamentsRepository.AddPooledBeatmapsAsync(id, osuBeatmapIds));

    public async Task<ICollection<BeatmapDTO>> GetPooledBeatmapsAsync(int id) =>
        mapper.Map<ICollection<BeatmapDTO>>(await tournamentsRepository.GetPooledBeatmapsAsync(id));

    public async Task DeletePooledBeatmapsAsync(int id, ICollection<int> beatmapIds) =>
        await tournamentsRepository.DeletePooledBeatmapsAsync(id, beatmapIds);

    public async Task DeletePooledBeatmapsAsync(int id) =>
        await tournamentsRepository.DeletePooledBeatmapsAsync(id);
}
