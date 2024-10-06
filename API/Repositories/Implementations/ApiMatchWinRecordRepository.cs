using API.DTOs;
using API.Repositories.Interfaces;
using Database;
using Database.Entities;
using Database.Enums;
using Database.Repositories.Implementations;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class ApiMatchWinRecordRepository(
    OtrContext context,
    IPlayersRepository playersRepository
    ) : MatchWinRecordRepository(context), IApiMatchWinRecordRepository
{
    private readonly OtrContext _context = context;

    public async Task<IEnumerable<PlayerFrequencyDTO>> GetFrequentTeammatesAsync(
        int playerId,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null,
        int limit = 5
    )
    {
        List<MatchWinRecord> redTeams = await _context
            .MatchWinRecords.Where(x =>
                x.Match.Tournament.Ruleset == ruleset
                && x.Match.StartTime >= dateMin
                && x.Match.StartTime <= dateMax
                && x.WinnerRoster.Contains(playerId)
            )
            .ToListAsync();

        List<MatchWinRecord> blueTeams = await _context
            .MatchWinRecords.Where(x =>
                x.Match.Tournament.Ruleset == ruleset
                && x.Match.StartTime >= dateMin
                && x.Match.StartTime <= dateMax
                && x.LoserRoster.Contains(playerId)
            )
            .ToListAsync();

        // Produce an ordered list of player ids and their frequency of being a teammate with the player in question
        return redTeams
            .Concat(blueTeams)
            .SelectMany(x => x.WinnerRoster.Concat(x.LoserRoster))
            .Where(x => x != playerId)
            .GroupBy(x => x)
            .OrderByDescending(x => x.Count())
            .Take(limit)
            .Select(x => new PlayerFrequencyDTO
            {
                PlayerId = x.Key,
                Frequency = x.Count(),
                OsuId = playersRepository.GetOsuIdAsync(x.Key).GetAwaiter().GetResult(),
                Username = playersRepository.GetUsernameAsync(x.Key).GetAwaiter().GetResult()
            });
    }

    public async Task<IEnumerable<PlayerFrequencyDTO>> GetFrequentOpponentsAsync(
        int playerId,
        Ruleset ruleset,
        DateTime? dateMin = null,
        DateTime? dateMax = null,
        int limit = 5
    )
    {
        List<MatchWinRecord> matchData = await _context
            .MatchWinRecords.Where(x =>
                (!x.WinnerRoster.Contains(playerId) && x.LoserRoster.Contains(playerId))
                || (!x.LoserRoster.Contains(playerId) && x.WinnerRoster.Contains(playerId))
            )
            .Where(x =>
                x.Match.Tournament.Ruleset == ruleset
                && x.Match.StartTime >= dateMin
                && x.Match.StartTime <= dateMax
            )
            .ToListAsync();

        IEnumerable<int> filteredData = matchData
            .Where(x => x.WinnerRoster.Contains(playerId))
            .SelectMany(x => x.LoserRoster)
            .Concat(matchData.Where(x => x.LoserRoster.Contains(playerId)).SelectMany(x => x.WinnerRoster));

        return filteredData
            .GroupBy(x => x)
            .OrderByDescending(x => x.Count())
            .Take(limit)
            .Select(x => new PlayerFrequencyDTO
            {
                PlayerId = x.Key,
                Frequency = x.Count(),
                OsuId = playersRepository.GetOsuIdAsync(x.Key).GetAwaiter().GetResult(),
                Username = playersRepository.GetUsernameAsync(x.Key).GetAwaiter().GetResult()
            });
    }
}
