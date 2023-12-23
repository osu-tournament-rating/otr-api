using API.DTOs;
using API.Entities;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class MatchWinRecordRepository : RepositoryBase<MatchWinRecord>, IMatchWinRecordRepository
{
	private readonly OtrContext _context;
	public MatchWinRecordRepository(OtrContext context) : base(context) { _context = context; }

	public async Task BatchInsertAsync(IEnumerable<MatchWinRecordDTO> postBody)
	{
		foreach (var item in postBody)
		{
			var record = new MatchWinRecord
			{
				MatchId = item.MatchId,
				Team1 = item.Team1,
				Team2 = item.Team2,
				Team1Points = item.Team1Points,
				Team2Points = item.Team2Points,
				WinnerTeam = item.WinnerTeam,
				LoserTeam = item.LoserTeam
			};

			await _context.MatchWinRecords.AddAsync(record);
		}

		await _context.SaveChangesAsync();
	}

	public async Task TruncateAsync()
	{
		await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE match_win_records RESTART IDENTITY");
	}
}