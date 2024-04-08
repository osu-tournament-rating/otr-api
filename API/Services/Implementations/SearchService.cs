using API.DTOs;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using API.Utilities;

namespace API.Services.Implementations;

public class SearchService(
    ITournamentsRepository tournamentsRepository,
    IMatchesRepository matchesRepository,
    IPlayerRepository playerRepository,
    IUrlHelperService urlHelperService
    ) : ISearchService
{
    public async Task<SearchResponseCollectionDTO?> SearchByNameAsync(string searchKey)
    {
        var result = new SearchResponseCollectionDTO
        {
            Tournaments = (await SearchTournamentsByNameAsync(searchKey)).ToArray(),
            Matches = (await SearchMatchesByNameAsync(searchKey)).ToArray(),
            Players = (await SearchPlayersByNameAsync(searchKey)).ToArray()
        };

        return result.Tournaments.Length == 0 && result.Matches.Length == 0 && result.Players.Length == 0
            ? null
            : result;
    }

    private async Task<IEnumerable<SearchResponseDTO>> SearchTournamentsByNameAsync(string tournamentName)
    {
        var tournaments = (await tournamentsRepository.SearchAsync(tournamentName)).ToList();
        return tournaments.Select(tournament => new SearchResponseDTO
        {
            Text = tournament.Name,
            Url = urlHelperService.Action(CreatedAtRouteValuesHelper.GetTournament(tournament.Id))
        });
    }

    private async Task<IEnumerable<SearchResponseDTO>> SearchMatchesByNameAsync(string matchName)
    {
        var matches = (await matchesRepository.SearchAsync(matchName)).ToList();
        return matches.Select(match => new SearchResponseDTO
        {
            Text = match.Name ?? match.MatchId.ToString(),
            Url = urlHelperService.Action(CreatedAtRouteValuesHelper.GetMatch(match.Id))
        });
    }

    private async Task<IEnumerable<SearchResponseDTO>> SearchPlayersByNameAsync(string username)
    {
        var players = (await playerRepository.SearchAsync(username)).ToList();
        return players.Select(player => new SearchResponseDTO
        {
            Text = player.Username ?? "<Unknown>",
            Url = urlHelperService.Action(CreatedAtRouteValuesHelper.GetPlayer(player.Id)),
            Thumbnail = $"a.ppy.sh/{player.OsuId}"
        });
    }
}
