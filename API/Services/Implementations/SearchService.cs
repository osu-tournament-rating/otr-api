using API.DTOs;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

namespace API.Services.Implementations;

public class SearchService(ITournamentsRepository tournamentsRepository, IMatchesRepository matchesRepository, IPlayerRepository playerRepository) : ISearchService
{
    private readonly ITournamentsRepository _tournamentsRepository = tournamentsRepository;
    private readonly IMatchesRepository _matchesRepository = matchesRepository;
    private readonly IPlayerRepository _playerRepository = playerRepository;

    public async Task<List<SearchResponseDTO>?> SearchByNameAsync(string? tournamentName, string? matchName, string? username)
    {
        IEnumerable<SearchResponseDTO>? returnList = await SearchTournamentsByNameAsync(tournamentName);

        if (returnList is not null)
        {
            return returnList.ToList();
        }

        returnList = await SearchMatchesByNameAsync(matchName);

        if (returnList is not null)
        {
            return returnList.ToList();
        }

        returnList = await SearchPlayersByNameAsync(username);

        if (returnList is not null)
        {
            return returnList.ToList();
        }

        return null;
    }

    private async Task<IEnumerable<SearchResponseDTO>?> SearchTournamentsByNameAsync(string? tournamentName)
    {
        var returnList = new List<SearchResponseDTO>();

        if (tournamentName is null)
        {
            return null;
        }

        var tournaments = (await _tournamentsRepository.SearchAsync(tournamentName)).ToList();

        if (tournaments.Count == 0)
        {
            return null;
        }

        returnList.AddRange(tournaments.Select(tournament => new SearchResponseDTO() { Text = tournament.Name, Url = $"/tournaments/{tournament.Id}" }));

        return returnList;
    }

    private async Task<IEnumerable<SearchResponseDTO>?> SearchMatchesByNameAsync(string? matchName)
    {
        var returnList = new List<SearchResponseDTO>();

        if (matchName is null)
        {
            return null;
        }

        var matches = (await _matchesRepository.SearchAsync(matchName)).ToList();

        if (matches.Count == 0)
        {
            return null;
        }

        returnList.AddRange(matches.Select(match => new SearchResponseDTO() { Text = match.Id.ToString(), Url = $"/matches/{match.MatchId}" }));

        return returnList;
    }

    private async Task<IEnumerable<SearchResponseDTO>?> SearchPlayersByNameAsync(string? username)
    {
        var returnList = new List<SearchResponseDTO>();

        if (username is null)
        {
            return null;
        }

        var players = (await _playerRepository.SearchAsync(username)).ToList();

        if (players.Count == 0)
        {
            return null;
        }

        returnList.AddRange(players.Select(player => new SearchResponseDTO() { Text = player.Username ?? "<Unknown>", Url = $"/users/{player.Id}", Thumbnail = $"a.ppy.sh/{player.OsuId}" }));

        return returnList;
    }
}
