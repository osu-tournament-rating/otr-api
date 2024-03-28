using API.DTOs;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

namespace API.Services.Implementations;

public class SearchService(ITournamentsRepository tournamentsRepository, IMatchesRepository matchesRepository, IPlayerRepository playerRepository) : ISearchService
{
    private readonly ITournamentsRepository _tournamentsRepository = tournamentsRepository;
    private readonly IMatchesRepository _matchesRepository = matchesRepository;
    private readonly IPlayerRepository _playerRepository = playerRepository;

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

        var tournaments = (await _tournamentsRepository.SearchAsync(tournamentName)).ToList();
        return tournaments.Select(tournament => new SearchResponseDTO() { Type = "Tournament", Text = tournament.Name, Url = $"/tournaments/{tournament.Id}" });
    }

    private async Task<IEnumerable<SearchResponseDTO>> SearchMatchesByNameAsync(string? matchName)
    {
        if (matchName is null)
        {
            return new List<SearchResponseDTO>(); ;
        }

        var matches = (await _matchesRepository.SearchAsync(matchName)).ToList();
        return matches.Select(match => new SearchResponseDTO() { Type = "Match", Text = match.MatchId.ToString(), Url = $"/matches/{match.Id}" });
    }

    private async Task<IEnumerable<SearchResponseDTO>> SearchPlayersByNameAsync(string? username)
    {
        if (username is null)
        {
            return new List<SearchResponseDTO>(); ;
        }

        var players = (await _playerRepository.SearchAsync(username)).ToList();
        return players.Select(player => new SearchResponseDTO() { Type = "Player", Text = player.Username ?? "<Unknown>", Url = $"/users/{player.Id}", Thumbnail = $"a.ppy.sh/{player.OsuId}" });
    }
}
