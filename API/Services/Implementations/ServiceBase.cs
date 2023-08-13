using API.Configurations;
using API.Entities.Bases;
using API.Services.Interfaces;
using Dapper;
using Npgsql;

namespace API.Services.Implementations;

public class ServiceBase<T> : IService<T> where T : class, IEntity
{
	private readonly ILogger _logger;

	protected ServiceBase(IDbCredentials dbCredentials, ILogger logger)
	{
		_logger = logger;
		ConnectionString = dbCredentials.ConnectionString;
	}

	public string ConnectionString { get; }

	public async Task<int?> CreateAsync(T entity)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.InsertAsync(entity);
		}
	}

	public async Task<T?> GetAsync(int id)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.GetAsync<T>(id);
		}
	}

	public async Task<int?> UpdateAsync(T entity)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			try
			{
				return await connection.UpdateAsync(entity);
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Failed to update entity {Entity}", entity);
				return null;
			}
		}
	}

	public async Task<int?> DeleteAsync(int id)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			try
			{
				return await connection.DeleteAsync(id);
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Failed to delete entity with id {Id}", id);
				return null;
			}
		}
	}

	public async Task<IEnumerable<T>?> GetAllAsync()
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			// May consider not calling .ToList for performance reasons
			var res = (await connection.GetListAsync<T>()).ToList();
			return res.Any() ? res : null;
		}
	}

	public async Task<bool> ExistsAsync(int id)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			try
			{
				return await connection.QueryFirstAsync<T>("SELECT * FROM [dbo].[config] WHERE [Id] = @Id", new { Id = id }) != null;
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Failed to check for existing entity with id {Id}", id);
				return false;
			}
		}
	}
}