using API.Entities;
using API.Osu.Multiplayer;

namespace API.Services.Interfaces;

public interface IMatchesService : IService<Entities.Match>
{
	Task<Entities.Match?> GetByLobbyIdAsync(long matchId);
	Task<IEnumerable<Entities.Match>?> GetAllPendingVerificationAsync();
	Task<Entities.Match?> GetFirstPendingOrDefaultAsync();
	Task<IEnumerable<long>> CheckExistingAsync(IEnumerable<long> matchIds);
	/// <summary>
	/// Used to queue up matches for verification.
	/// </summary>
	/// <returns>Number of rows inserted</returns>
	Task<int> InsertFromIdBatchAsync(IEnumerable<Entities.Match> matches);
	/// <summary>
	/// Creates a match if it doesn't already exist.
	/// </summary>
	/// <param name="match"></param>
	/// <returns>Primary key if created, otherwise null</returns>
	Task<int?> CreateIfNotExistsAsync(Entities.Match match);
	Task<bool> CreateFromApiMatchAsync(OsuApiMatchData osuMatch);
	Task<int> UpdateVerificationStatusAsync(long matchId, VerificationStatus status, MatchVerificationSource source, string? info = null);
	Task<IEnumerable<Entities.Match>> GetForPlayerAsync(int playerId);
}