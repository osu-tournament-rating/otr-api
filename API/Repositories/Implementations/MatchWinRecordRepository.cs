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
				TeamBlue = item.TeamBlue,
				TeamRed = item.TeamRed,
				BluePoints = item.BluePoints,
				RedPoints = item.RedPoints,
				WinnerTeam = item.WinnerTeam,
				LoserTeam = item.LoserTeam,
				MatchType = (Entities.MatchType?)item.MatchType
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