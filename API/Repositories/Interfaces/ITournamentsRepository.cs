using API.Controllers;
using API.Entities;

namespace API.Repositories.Interfaces;

public interface ITournamentsRepository : IRepository<Tournament>
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
	public Task<Tournament> CreateOrUpdateAsync(BatchWrapper wrapper, bool updateExisting = false);
	public Task<bool> ExistsAsync(string name, int mode);
	/// <summary>
	/// Returns a list of tournaments the player has participated in
	/// </summary>
	Task<IEnumerable<Tournament>> GetForPlayerAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax);

	/// <summary>
	/// Returns the top <see cref="count"/> tournaments in which the player had the best average match cost across matches participated
	/// </summary>
	/// <param name="count">The number of tournaments to return</param>
	/// <returns>A list of <see cref="count"/> tournaments ordered by the player's average match cost, descending</returns>
	Task<IEnumerable<Tournament>> GetTopPerformancesAsync(int count, int playerId, int mode, DateTime dateMin,
		DateTime dateMax);

	Task<IEnumerable<Tournament>> GetAllAsync();
}