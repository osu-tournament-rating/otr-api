using API.Entities;
using API.Osu;
using API.Repositories.Interfaces;
using API.Utilities;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class GamesRepository : RepositoryBase<Game>, IGamesRepository
{
	private readonly OtrContext _context;

	public GamesRepository(OtrContext context) : base(context)
	{
		_context = context;
	}

	public override async Task<int> UpdateAsync(Game game)
	{
		game.Updated = DateTime.UtcNow;
		return await base.UpdateAsync(game);
	}

	public async Task<int> CountGameWinsAsync(long osuPlayerId, int mode, DateTime fromTime)
	{
		/**
		 * This one's a little tricky. For purely head-to-head matches, which are ideally 1v1,
		 * we simply return the number of matches where the player's score is higher than the opponent's.
		 *
		 * For team matches, we need to do a little more work. We need to find the player's team, and then
		 * find the opposing team. Then, we need to sum both team's scores. If the sum of the player's team
		 * is greater than the opponent, then this is counted as a win.
		 *
		 * TODO: Implement accuracy win condition
		 */
		int wins = 0;
		var games = await _context.Games
		                          .WhereVerified()
		                          .Include(x => x.MatchScores)
		                          .ThenInclude(x => x.Player)
		                          .After(fromTime)
		                          .Where(x => x.MatchScores.Any(y => y.Player.OsuId == osuPlayerId) && x.PlayMode == mode)
		                          .ToListAsync();

		// Process HeadToHead (includes 1v1 team games)
		var headToHeadGames = games
		                      .Where(x => x.MatchScores.Count == 2)
		                      .ToList();

		foreach (var game in headToHeadGames)
		{
			try
			{
				var playerScore = game.MatchScores.First(x => x.Player.OsuId == osuPlayerId);
				var opponentScore = game.MatchScores.First(x => x.Player.OsuId != osuPlayerId);
				if (playerScore.Score > opponentScore.Score)
				{
					wins++;
				}
			}
			catch (Exception)
			{
				//
			}
		}

		// Team games
		var teamGames = games.Where(x => x.TeamTypeEnum != OsuEnums.TeamType.HeadToHead && x.MatchScores.Count >= 4).ToList();
		foreach (var game in teamGames)
		{
			try
			{
				int playerTeam = game.MatchScores.First(x => x.Player.OsuId == osuPlayerId).Team;
				int opponentTeam = game.MatchScores.First(x => x.Player.OsuId != osuPlayerId && x.Team != playerTeam).Team;

				long playerTeamScores = game.MatchScores.Where(x => x.Team == playerTeam).Sum(x => x.Score);
				long opponentTeamScores = game.MatchScores.Where(x => x.Team == opponentTeam).Sum(x => x.Score);

				if (playerTeamScores > opponentTeamScores)
				{
					wins++;
				}
			}
			catch (Exception)
			{
				//
			}
		}

		return wins;
	}

	public async Task<string?> MostPlayedTeammateNameAsync(long osuPlayerId, int mode, DateTime fromDate)
	{
		var verifiedMatchScores = _context.MatchScores.WhereVerified();
		var teammateMatchScores = verifiedMatchScores.WhereTeammate(osuPlayerId)
		                                             .WhereMode(mode)
		                                             .After(fromDate);
    
		var teammateNames = await (from oms in teammateMatchScores
		                           join p in _context.Players on oms.PlayerId equals p.Id
		                           where p.Username != null
		                           select p.Username).ToListAsync();

		var mostPlayedTeammate = teammateNames.GroupBy(x => x).MaxBy(g => g.Count());

		return mostPlayedTeammate?.Key;
	}

	public async Task<string?> MostPlayedOpponentNameAsync(long osuPlayerId, int mode, DateTime fromDate)
	{
		var verifiedMatchScores = _context.MatchScores.WhereVerified();
		var opponentMatchScores = verifiedMatchScores.WhereOpponent(osuPlayerId)
		                                             .WhereMode(mode)
		                                             .After(fromDate);
    
		var opponentNames = await (from oms in opponentMatchScores
		                           join p in _context.Players on oms.PlayerId equals p.Id
		                           where p.Username != null
		                           select p.Username).ToListAsync();

		var mostPlayedOpponent = opponentNames.GroupBy(x => x).MaxBy(g => g.Count());

		return mostPlayedOpponent?.Key;
	}
}