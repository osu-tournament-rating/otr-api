using API.DTOs;
using API.Entities;
using API.Osu;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

namespace API.Services.Implementations;

public class SearchService(
    ITournamentsRepository tournamentsRepository,
    IMatchesRepository matchesRepository,
    IPlayerRepository playerRepository
    ) : ISearchService
{
    public async Task<SearchResponseCollectionDTO> SearchByNameAsync(string searchKey) =>
        new SearchResponseCollectionDTO
        {
            Tournaments = (await SearchTournamentsByNameAsync(searchKey)).ToList(),
            Matches = (await SearchMatchesByNameAsync(searchKey)).ToList(),
            Players = (await SearchPlayersByNameAsync(searchKey)).ToList()
        };

    private async Task<IEnumerable<TournamentSearchResultDTO>> SearchTournamentsByNameAsync(string tournamentName)
    {
        var tournaments = (await tournamentsRepository.SearchAsync(tournamentName)).ToList();
        return tournaments.Select(tournament => new TournamentSearchResultDTO
        {
            Id = tournament.Id,
            Ruleset = (OsuEnums.Mode)tournament.Mode,
            TeamSize = tournament.TeamSize,
            Name = tournament.Name,
        });
    }

    private async Task<IEnumerable<MatchSearchResultDTO>> SearchMatchesByNameAsync(string matchName)
    {
        var matches = (await matchesRepository.SearchAsync(matchName)).ToList();
        return matches.Select(match => new MatchSearchResultDTO
        {
            Id = match.Id,
            MatchId = match.MatchId,
            Name = match.Name
        });
    }

    private async Task<IEnumerable<PlayerSearchResultDTO>> SearchPlayersByNameAsync(string username)
    {
        var players = (await playerRepository.SearchAsync(username)).ToList();
        return players.Select(player =>
        {
            // TODO: Use Player.Ruleset for Rating and OsuGlobalRank
            BaseStats? rating = player.Ratings.FirstOrDefault(r => r.Mode == (int)OsuEnums.Mode.Standard);
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
