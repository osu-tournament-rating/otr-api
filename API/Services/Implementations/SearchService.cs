using API.DTOs;
using API.Entities;
using API.Osu;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using AutoMapper;

namespace API.Services.Implementations;

public class SearchService(
    ITournamentsRepository tournamentsRepository,
    IMatchesRepository matchesRepository,
    IPlayerRepository playerRepository,
    IMapper mapper
    ) : ISearchService
{
    public async Task<SearchResponseCollectionDTO> SearchByNameAsync(string searchKey) =>
        new()
        {
            Tournaments = (await SearchTournamentsByNameAsync(searchKey)).ToList(),
            Matches = (await SearchMatchesByNameAsync(searchKey)).ToList(),
            Players = (await SearchPlayersByNameAsync(searchKey)).ToList()
        };

    private async Task<IEnumerable<TournamentSearchResultDTO>> SearchTournamentsByNameAsync(string tournamentName)
    {
        IEnumerable<Tournament> tournaments = await tournamentsRepository.SearchAsync(tournamentName);
        return tournaments.Select(mapper.Map<TournamentSearchResultDTO>);
    }

    private async Task<IEnumerable<MatchSearchResultDTO>> SearchMatchesByNameAsync(string matchName)
    {
        var matches = (await matchesRepository.SearchAsync(matchName)).ToList();
        return matches.Select(mapper.Map<MatchSearchResultDTO>);
    }

    private async Task<IEnumerable<PlayerSearchResultDTO>> SearchPlayersByNameAsync(string username)
    {
        var players = (await playerRepository.SearchAsync(username)).ToList();
        return players.Select(player =>
        {
            BaseStats? rating = player.Ratings
                .FirstOrDefault(r => r.Mode == (player.Ruleset ?? OsuEnums.Ruleset.Standard));
            return new PlayerSearchResultDTO
            {
                Id = player.Id,
                OsuId = player.OsuId,
                Rating = rating?.Rating,
                GlobalRank = rating?.GlobalRank,
                Username = player.Username,
                Thumbnail = $"a.ppy.sh/{player.OsuId}"
            };
        });
    }
}
