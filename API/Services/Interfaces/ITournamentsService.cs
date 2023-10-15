using API.Controllers;
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
	/// <summary>
	/// Creates or udpates a tournament from a web submission.
	/// </summary>
	/// <param name="wrapper">The user input required for this tournament</param>
	/// <param name="updateExisting">Whether to overwrite values for an existing occurrence of this tournament</param>
	/// <returns></returns>
	public Task<Tournament> CreateAsync(BatchWrapper wrapper, bool updateExisting = false);

	/// <summary>
	/// Checks whether the tournament with a given name exists
	/// </summary>
	/// <param name="name"></param>
	/// <returns>true if the tournament exists</returns>
	public Task<bool> ExistsAsync(string name, int mode);
	
	// TODO: Add an ExistsAsync(long forumId) check
}