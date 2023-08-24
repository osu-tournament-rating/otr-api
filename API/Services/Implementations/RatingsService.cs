using API.Configurations;
using API.Entities;
using API.Services.Interfaces;
using Dapper;
using Npgsql;

namespace API.Services.Implementations;

public class RatingsService : ServiceBase<Rating>, IRatingsService
{
	private readonly ILogger<RatingsService> _logger;
	private readonly IPlayerService _playerService;

	public RatingsService(ICredentials credentials, ILogger<RatingsService> logger, IPlayerService playerService) : base(credentials, logger)
	{
		_logger = logger;
		_playerService = playerService;
	}

	public async Task<IEnumerable<Rating>> GetAllForPlayerAsync(int id)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryAsync<Rating>("SELECT * FROM ratings WHERE player_id = @PlayerId", new { PlayerId = id });
		}
	}

	public override async Task<int?> UpdateAsync(Rating entity)
	{
		using(var connection = new NpgsqlConnection(ConnectionString))
		{
			// First, copy the current state of the entity to the history table.
			var history = new RatingHistory
			{
				PlayerId = entity.PlayerId,
				Mu = entity.Mu,
				Sigma = entity.Sigma,
				Created = DateTime.UtcNow,
				Mode = entity.Mode
			};

			try
			{
				await connection.ExecuteAsync("INSERT INTO ratinghistories (player_id, mu, sigma, created, mode, match_data_id) VALUES (@PlayerId, @Mu, @Sigma, @Created, @Mode, @MatchDataId)", history);
				return await connection.ExecuteAsync("UPDATE ratings SET mu = @Mu, sigma = @Sigma WHERE player_id = @PlayerId AND mode = @Mode", entity);
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Failed to update rating for player {PlayerId} in mode {Mode}", entity.PlayerId, entity.Mode);
				return null;
			}
		}
	}

	public async Task UpdateBatchAsync(IEnumerable<Rating> ratings)
	{
		ratings = ratings.ToList();
		var players = await _playerService.GetAllAsync();
		var ids = players!.ToDictionary(x => x.OsuId, x => x.Id);
		
		foreach (var r in ratings)
		{
			// Update the playerId to be that from the database, not from osu!.
			try
			{
				int id = ids[r.PlayerId];
				r.PlayerId = id;
			}
			catch (Exception e)
			{
				_logger.LogWarning(e, "Player {@Player} has a rating update but could not be found", r);
			}
			
		}
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			_logger.LogInformation("A batch of {Count} ratings are being updated, this will take a while...", ratings.Count());
			await connection.ExecuteAsync("UPDATE ratings SET mu = @Mu, sigma = @Sigma WHERE player_id = @PlayerId AND mode = @Mode",
				ratings);
			_logger.LogInformation("Ratings batch update complete");
		}
	}

	public async Task<int> InsertOrUpdateForPlayerAsync(int playerId, Rating rating)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			if (playerId != rating.PlayerId)
			{
				throw new ArgumentException("The player id in the rating object does not match the id in the route");
			}

			rating.Updated = DateTime.UtcNow; // This doesn't work for some reason...?
			return await connection.ExecuteAsync("INSERT INTO ratings (player_id, mu, sigma, mode, updated) VALUES (@PlayerId, @Mu, @Sigma, @Mode, @Updated) " +
			                                     "ON CONFLICT (player_id, mode) DO UPDATE SET mu = @Mu, sigma = @Sigma, updated = @Updated", rating);
		}
	}

	public async Task<int?> BatchInsertOrUpdateAsync(IEnumerable<Rating> ratings)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.ExecuteAsync("INSERT INTO ratings (player_id, mu, sigma, mode, updated) VALUES (@PlayerId, @Mu, @Sigma, @Mode, @Updated) " +
			                                     "ON CONFLICT (player_id, mode) DO UPDATE SET mu = @Mu, sigma = @Sigma, updated = @Updated", ratings);
		}
	}
}