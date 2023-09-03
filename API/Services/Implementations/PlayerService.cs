using API.Configurations;
using API.Entities;
using API.Services.Interfaces;
using Dapper;
using Npgsql;

namespace API.Services.Implementations;

public class PlayerService : ServiceBase<Player>, IPlayerService
{
	public PlayerService(ICredentials credentials, ILogger<PlayerService> logger) : base(credentials, logger) {}

	public async Task<Player?> GetByOsuIdAsync(long osuId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QuerySingleOrDefaultAsync<Player>("SELECT * FROM players WHERE osu_id = @OsuId", new { OsuId = osuId });
		}
	}

	public async Task<IEnumerable<Player>> GetByOsuIdsAsync(IEnumerable<long> osuIds)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryAsync<Player>("SELECT * FROM players WHERE osu_id = ANY(@OsuIds)", new { OsuIds = osuIds });
		}
	}

	// public async Task<IEnumerable<Player>> GetByOsuIdsAsync(IEnumerable<long> osuIds)
	// {
	// 	using (var connection = new NpgsqlConnection(ConnectionString))
	// 	{
	// 		const string sql = @"SELECT * FROM players p
 //        INNER JOIN ratings r on p.id = r.player_id
 //        INNER JOIN ratinghistories h on p.id = h.player_id
 //        INNER JOIN match_scores ms on p.id = ms.player_id
 //        INNER JOIN games g on ms.game_id = g.id
 //        INNER JOIN matches m on g.match_id = m.id WHERE osu_id = ANY(@OsuIds)";
 //
	// 		var playerDictionary = new Dictionary<long, Player>();
	// 		var ratingIds = new HashSet<(int, string)>();  // PlayerId, Mode
	// 		var matchScoreIds = new HashSet<(int, int)>(); // GameId, PlayerId
	// 		var matchDictionary = new Dictionary<int, Match>();
 //
	// 		await connection.QueryAsync<Player, Rating, RatingHistory, MatchScore, Game, Match, Player>(sql, (player, rating, ratingHistory, matchScore,
	// 			game, match) =>
	// 		{
	// 			if (!playerDictionary.TryGetValue(player.Id, out var currentPlayer))
	// 			{
	// 				currentPlayer = player;
	// 				currentPlayer.Ratings = new List<Rating>();
	// 				currentPlayer.RatingHistories = new List<RatingHistory>();
	// 				currentPlayer.Matches = new List<Match>();
	// 				playerDictionary.Add(currentPlayer.Id, currentPlayer);
	// 			}
 //
	// 			if (ratingIds.Add((rating.PlayerId, rating.Mode)))
	// 			{
	// 				currentPlayer.Ratings.Add(rating);
	// 			}
 //
	// 			if (matchScoreIds.Add((matchScore.GameId, matchScore.PlayerId)))
	// 			{
	// 				currentPlayer.RatingHistories.Add(ratingHistory);
	// 			}
 //
	// 			if (!matchDictionary.TryGetValue(match.Id, out var currentMatch))
	// 			{
	// 				currentMatch = match;
	// 				currentMatch.Games = new List<Game>();
	// 				matchDictionary.Add(currentMatch.Id, currentMatch);
	// 				currentPlayer.Matches.Add(currentMatch);
	// 			}
 //
	// 			currentMatch.Games.Add(game);
 //
	// 			return currentPlayer;
	// 		}, new { OsuIds = osuIds }, splitOn: "id, id, id, id, id");
 //
	// 		return playerDictionary.Values;
	// 	}
	// }

	public async Task<int> GetIdByOsuIdAsync(long osuId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QuerySingleOrDefaultAsync<int>("SELECT id FROM players WHERE osu_id = @OsuId", new { OsuId = osuId });
		}
	}

	public async Task<long> GetOsuIdByIdAsync(int id)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QuerySingleOrDefaultAsync<long>("SELECT osu_id FROM players WHERE id = @Id", new { Id = id });
		}
	}

	public async Task<Dictionary<long, int>> GetIdsByOsuIdsAsync(IEnumerable<long> osuIds)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			var results = await connection.QueryAsync<(long osuId, int id)>("SELECT osu_id, id FROM players WHERE osu_id = ANY(@OsuIds)", new { OsuIds = osuIds.ToArray() });
			return results.ToDictionary(x => x.osuId, x => x.id);
		}
	}
}