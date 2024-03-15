using System.Diagnostics.CodeAnalysis;
using API.DTOs;
using API.Entities;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class MatchWinRecordRepository(OtrContext context, IPlayerRepository playerRepository) : RepositoryBase<MatchWinRecord>(context), IMatchWinRecordRepository
{
    private readonly OtrContext _context = context;
    private readonly IPlayerRepository _playerRepository = playerRepository;

    public async Task BatchInsertAsync(IEnumerable<MatchWinRecordDTO> postBody)
    {
        foreach (MatchWinRecordDTO item in postBody)
        {
            var record = new MatchWinRecord
            {
                MatchId = item.MatchId,
                TeamBlue = item.TeamBlue,
                TeamRed = item.TeamRed,
                BluePoints = item.BluePoints,
                RedPoints = item.RedPoints,
                WinnerTeam = item.WinnerTeam,
                LoserTeam = item.LoserTeam,
                MatchType = (Enums.MatchType?)item.MatchType
            };

            await _context.MatchWinRecords.AddAsync(record);
        }

        await _context.SaveChangesAsync();
    }

    public async Task TruncateAsync() =>
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE match_win_records RESTART IDENTITY");

    public async Task<IEnumerable<PlayerFrequencyDTO>> GetFrequentTeammatesAsync(
        int playerId,
        int mode,
        DateTime? dateMin = null,
        DateTime? dateMax = null,
        int limit = 5
    )
    {
        List<MatchWinRecord> redTeams = await _context
            .MatchWinRecords.Where(x =>
                x.Match.Tournament.Mode == mode
                && x.Match.StartTime >= dateMin
                && x.Match.StartTime <= dateMax
                && x.TeamRed.Contains(playerId)
            )
            .ToListAsync();

        List<MatchWinRecord> blueTeams = await _context
            .MatchWinRecords.Where(x =>
                x.Match.Tournament.Mode == mode
                && x.Match.StartTime >= dateMin
                && x.Match.StartTime <= dateMax
                && x.TeamBlue.Contains(playerId)
            )
            .ToListAsync();

        // Produce an ordered list of player ids and their frequency of being a teammate with the player in question
        return redTeams
            .Concat(blueTeams)
            .SelectMany(x => x.TeamRed.Concat(x.TeamBlue))
            .Where(x => x != playerId)
            .GroupBy(x => x)
            .OrderByDescending(x => x.Count())
            .Take(limit)
            .Select(x => new PlayerFrequencyDTO
            {
                PlayerId = x.Key,
                Frequency = x.Count(),
                OsuId = _playerRepository.GetOsuIdAsync(x.Key).GetAwaiter().GetResult(),
                Username = _playerRepository.GetUsernameAsync(x.Key).GetAwaiter().GetResult()
            });
    }

    public async Task<IEnumerable<PlayerFrequencyDTO>> GetFrequentOpponentsAsync(
        int playerId,
        int mode,
        DateTime? dateMin = null,
        DateTime? dateMax = null,
        int limit = 5
    )
    {
        List<MatchWinRecord> matchData = await _context
            .MatchWinRecords.Where(x =>
                (!x.TeamRed.Contains(playerId) && x.TeamBlue.Contains(playerId))
                || (!x.TeamBlue.Contains(playerId) && x.TeamRed.Contains(playerId))
            )
            .Where(x =>
                x.Match.Tournament.Mode == mode
                && x.Match.StartTime >= dateMin
                && x.Match.StartTime <= dateMax
            )
            .ToListAsync();

        IEnumerable<int> filteredData = matchData
            .Where(x => x.TeamRed.Contains(playerId))
            .SelectMany(x => x.TeamBlue)
            .Concat(matchData.Where(x => x.TeamBlue.Contains(playerId)).SelectMany(x => x.TeamRed));

        return filteredData
            .GroupBy(x => x)
            .OrderByDescending(x => x.Count())
            .Take(limit)
            .Select(x => new PlayerFrequencyDTO
            {
                PlayerId = x.Key,
                Frequency = x.Count(),
                OsuId = _playerRepository.GetOsuIdAsync(x.Key).GetAwaiter().GetResult(),
                Username = _playerRepository.GetUsernameAsync(x.Key).GetAwaiter().GetResult()
            });
    }
}
