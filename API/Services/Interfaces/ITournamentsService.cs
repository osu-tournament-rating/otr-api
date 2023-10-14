using API.Entities;

namespace API.Services.Interfaces;

public interface ITournamentsService : IService<Tournament>
{
	public Task<Tournament?> GetAsync(string name);
	/// <summary>
	/// A one-time-use operation: takes existing data from known matches, inserts them into the tournaments table, and links tournaments to matches.
	/// </summary>
	/// <returns></returns>
	public Task PopulateAndLinkAsync();
}