using API.Entities;

namespace API.Services.Interfaces;

public interface IMatchDataService : IService<MatchData>
{
	/// <summary>
	/// </summary>
	/// <returns></returns>
	Task<IEnumerable<MatchData>> GetFilteredDataAsync();

	Task<IEnumerable<MatchData>> GetAllForPlayerAsync(int playerId);
}