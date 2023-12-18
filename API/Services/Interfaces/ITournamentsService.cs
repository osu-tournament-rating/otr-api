using API.Controllers;
using API.DTOs;
using API.Entities;

namespace API.Services.Interfaces;

public interface ITournamentsService
{
	/// <summary>
	/// Creates or udpates a tournament from a web submission.
	/// </summary>
	/// <param name="wrapper">The user input required for this tournament</param>
	/// <param name="updateExisting">Whether to overwrite values for an existing occurrence of this tournament</param>
	/// <returns></returns>
	public Task<Tournament> CreateOrUpdateAsync(BatchWrapper wrapper, bool updateExisting = false);
	public Task<bool> ExistsAsync(string name, int mode);
	Task<IEnumerable<TournamentDTO>> GetAllAsync();
}