using API.DTOs;
using API.Entities;
using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using OsuSharp.Domain;

namespace API.Services.Implementations;

public class SearchService(ITournamentsRepository tournamentsRepository, IMatchesRepository matchesRepository, IPlayerRepository playerRepository) : ISearchService
{
    private readonly ITournamentsRepository _tournamentsRepository = tournamentsRepository;
    private readonly IMatchesRepository _matchesRepository = matchesRepository;
    private readonly IPlayerRepository _playerRepository = playerRepository;

    public async Task<List<SearchResponseDTO>?> SearchByNameAsync(string? tournamentName, string? matchName, string? username)
    {
        var returnList = new List<SearchResponseDTO>();

        if (!string.IsNullOrEmpty(tournamentName))
        {
            var tournaments = (await _tournamentsRepository.SearchAsync(tournamentName)).ToList();

            if (tournaments.Count == 0)
            {
                return null;
            }

            returnList.AddRange(tournaments.Select(tournament => new SearchResponseDTO() { Text = tournament.Name, Url = $"/tournaments/{tournament.Name}" }));

            return returnList;
        }

        if (!string.IsNullOrEmpty(matchName))
        {
            var matches = (await _matchesRepository.SearchAsync(matchName)).ToList();

            if (matches.Count == 0)
            {
                return null;
            }

            returnList.AddRange(matches.Select(match => new SearchResponseDTO() { Text = match.Id.ToString(), Url = $"/matches/{match.MatchId}" }));

            return returnList;
        }

        //Since this is the last check, checking the inverse so code looks a bit cleaner.
        if (string.IsNullOrEmpty(username))
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
