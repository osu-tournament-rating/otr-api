using API.Osu.Multiplayer;

namespace API.Services.Interfaces;

public interface IApiMatchService
{
	/// <summary>
	/// Inserts necessary data into the database from an osu! API match result.
	/// </summary>
	/// <param name="apiMatch">A single osu! match as represented from the API v1 wiki: https://github.com/ppy/osu-api/wiki#apiget_match</param>
	Task CreateFromApiMatchAsync(OsuApiMatchData apiMatch);
}