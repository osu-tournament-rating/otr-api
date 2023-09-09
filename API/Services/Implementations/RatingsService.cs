using API.Configurations;
using API.Models;
using API.Services.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace API.Services.Implementations;

public class RatingsService : ServiceBase<Rating>, IRatingsService
{
	private readonly ILogger<RatingsService> _logger;

	public RatingsService(ILogger<RatingsService> logger) : base(logger)
	{
		_logger = logger;
	}

	public async Task<IEnumerable<Rating>> GetForPlayerAsync(int playerId)
	{
		using (var context = new OtrContext())
		{
			return await context.Ratings.Where(x => x.PlayerId == playerId).ToListAsync();
		}
	}

	public override async Task<int> UpdateAsync(Rating entity)
	{
		using (var context = new OtrContext())
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

			await context.RatingHistories.AddAsync(history);
			return await base.UpdateAsync(entity);
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

	public async Task<IEnumerable<Rating>> GetAllAsync()
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryAsync<Rating>("SELECT * FROM ratings");
		}
	}

	public async Task TruncateAsync()
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			await connection.ExecuteAsync("TRUNCATE TABLE ratings");
		}
	}
}