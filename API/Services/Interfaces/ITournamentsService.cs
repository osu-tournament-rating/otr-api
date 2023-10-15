using API.Controllers;
using API.Entities;

namespace API.Services.Interfaces;

public interface ITournamentsService
{
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
	public Task<Tournament> CreateOrUpdateAsync(BatchWrapper wrapper, bool updateExisting = false);
	public Task<bool> ExistsAsync(string name, int mode);
}