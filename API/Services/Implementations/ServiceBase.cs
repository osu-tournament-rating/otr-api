using API.Configurations;
using API.Entities;
using API.Entities.Bases;
using API.Services.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace API.Services.Implementations;

public class ServiceBase<T> : IService<T> where T : class, IEntity 
{
	private readonly string _connectionString;
	protected ServiceBase(IDbCredentials dbCredentials) { _connectionString = dbCredentials.ConnectionString; }

	public async Task<int?> CreateAsync(T entity)
	{
		using (var connection = new SqlConnection(_connectionString))
		{
			return await connection.InsertAsync(entity);
		}
	}

	public async Task<T?> GetAsync(int id)
	{
		using (var connection = new SqlConnection(_connectionString))
		{
			return await connection.GetAsync<T>(id);
		}
	}
	
	public async Task<int?> UpdateAsync(T entity) => throw new NotImplementedException();
	public async Task<int?> DeleteAsync(int id) => throw new NotImplementedException();

	public async Task<IEnumerable<T>?> GetAllAsync()
	{
		using (var connection = new SqlConnection(_connectionString))
		{
			// May consider not calling .ToList for performance reasons
			var res = (await connection.GetListAsync<T>()).ToList();
			return res.Any() ? res : null;
		}
	}
	public async Task<bool> ExistsAsync(int id) => throw new NotImplementedException();
}