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
    public async Task<List<SearchResponseDTO>> SearchByNameAsync(string? tournamentName, string? matchName, string? username)
    {
        var returnList = (await SearchTournamentsByNameAsync(tournamentName)).ToList();

        returnList.AddRange(await SearchMatchesByNameAsync(matchName));
        returnList.AddRange(await SearchPlayersByNameAsync(username));

        return returnList;
    }

    private async Task<IEnumerable<SearchResponseDTO>> SearchTournamentsByNameAsync(string? tournamentName)
    {
        if (tournamentName is null)
        {
            return new List<SearchResponseDTO>();
        }

        var tournaments = (await tournamentsRepository.SearchAsync(tournamentName)).ToList();
        return tournaments.Select(tournament => new SearchResponseDTO
        {
            Type = "Tournament",
            Text = tournament.Name,
            Url = urlHelperService.Action(CreatedAtRouteValuesHelper.GetTournament(tournament.Id))
        });
    }

    private async Task<IEnumerable<SearchResponseDTO>> SearchMatchesByNameAsync(string? matchName)
    {
        if (matchName is null)
        {
            return new List<SearchResponseDTO>();
        }

        var matches = (await matchesRepository.SearchAsync(matchName)).ToList();
        return matches.Select(match => new SearchResponseDTO
        {
            Type = "Match",
            Text = match.MatchId.ToString(),
            Url = urlHelperService.Action(CreatedAtRouteValuesHelper.GetMatch(match.Id))
        });
    }

    private async Task<IEnumerable<SearchResponseDTO>> SearchPlayersByNameAsync(string? username)
    {
        if (username is null)
        {
            return new List<SearchResponseDTO>();
        }

        var players = (await playerRepository.SearchAsync(username)).ToList();
        return players.Select(player => new SearchResponseDTO
        {
            Type = "Player",
            Text = player.Username ?? "<Unknown>",
            Url = urlHelperService.Action(CreatedAtRouteValuesHelper.GetPlayer(player.Id)),
            Thumbnail = $"a.ppy.sh/{player.OsuId}"
        });
    }
}
