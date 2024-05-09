using System.Diagnostics.CodeAnalysis;
using API.DTOs;
using API.Entities;
using API.Osu.Enums;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class MatchWinRecordRepository(OtrContext context, IPlayerRepository playerRepository) : RepositoryBase<MatchWinRecord>(context), IMatchWinRecordRepository
{
    private readonly OtrContext _context = context;

    public async Task BatchInsertAsync(IEnumerable<MatchWinRecordDTO> postBody)
    {
        foreach (MatchWinRecordDTO item in postBody)
        {
            var record = new MatchWinRecord
            {
                MatchId = item.MatchId,
                LoserRoster = item.TeamBlue,
                WinnerRoster = item.TeamRed,
                LoserPoints = item.BluePoints,
                WinnerPoints = item.RedPoints,
                WinnerTeam = (Team?)item.WinnerTeam,
                LoserTeam = (Team?)item.LoserTeam,
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
                && x.WinnerRoster.Contains(playerId)
            )
            .ToListAsync();

        List<MatchWinRecord> blueTeams = await _context
            .MatchWinRecords.Where(x =>
                x.Match.Tournament.Mode == mode
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
                OsuId = playerRepository.GetOsuIdAsync(x.Key).GetAwaiter().GetResult(),
                Username = playerRepository.GetUsernameAsync(x.Key).GetAwaiter().GetResult()
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
                (!x.WinnerRoster.Contains(playerId) && x.LoserRoster.Contains(playerId))
                || (!x.LoserRoster.Contains(playerId) && x.WinnerRoster.Contains(playerId))
            )
            .Where(x =>
                x.Match.Tournament.Mode == mode
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
                OsuId = playerRepository.GetOsuIdAsync(x.Key).GetAwaiter().GetResult(),
                Username = playerRepository.GetUsernameAsync(x.Key).GetAwaiter().GetResult()
            });
    }
}
