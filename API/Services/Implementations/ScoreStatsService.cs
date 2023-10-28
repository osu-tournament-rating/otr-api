using API.DTOs;
using API.Osu;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

namespace API.Services.Implementations;

public class PlayerScoreStatsService : IPlayerScoreStatsService
{
	private readonly IMatchScoresRepository _scoresRepository;
	public PlayerScoreStatsService(IMatchScoresRepository scoresRepository)
	{
		_scoresRepository = scoresRepository;
	}
	
	public async Task<PlayerScoreStatsDTO> GetScoreStatsAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax)
	{
		const int modNm = (int)OsuEnums.Mods.None;
		const int modEz = (int)OsuEnums.Mods.Easy;
		const int modHd = (int)OsuEnums.Mods.Hidden;
		const int modHr = (int)OsuEnums.Mods.HardRock;
		const int modDt = (int)OsuEnums.Mods.DoubleTime;
		const int modFl = (int)OsuEnums.Mods.Flashlight;
		const int modHdHr = modHd | modHr;
		const int modHdDt = modHd | modDt;
		
		return new PlayerScoreStatsDTO
		{
			AverageScoreNM = await GetAverageModScoreAsync(playerId, mode, modNm, dateMin, dateMax),
			AverageScoreEZ = await GetAverageModScoreAsync(playerId, mode, modEz, dateMin, dateMax),
			AverageScoreHD = await GetAverageModScoreAsync(playerId, mode, modHd, dateMin, dateMax),
			AverageScoreHR = await GetAverageModScoreAsync(playerId, mode, modHr, dateMin, dateMax),
			AverageScoreDT = await GetAverageModScoreAsync(playerId, mode, modDt, dateMin, dateMax),
			AverageScoreFL = await GetAverageModScoreAsync(playerId, mode, modFl, dateMin, dateMax),
			AverageScoreHDHR = await GetAverageModScoreAsync(playerId, mode, modHdHr, dateMin, dateMax),
			AverageScoreHDDT = await GetAverageModScoreAsync(playerId, mode, modHdDt, dateMin, dateMax),
			CountPlayedNM = await GetCountModAsync(playerId, mode, modNm, dateMin, dateMax),
			CountPlayedEZ = await GetCountModAsync(playerId, mode, modEz, dateMin, dateMax),
			CountPlayedHD = await GetCountModAsync(playerId, mode, modHd, dateMin, dateMax),
			CountPlayedHR = await GetCountModAsync(playerId, mode, modHr, dateMin, dateMax),
			CountPlayedDT = await GetCountModAsync(playerId, mode, modDt, dateMin, dateMax),
			CountPlayedFL = await GetCountModAsync(playerId, mode, modFl, dateMin, dateMax),
			CountPlayedHDHR = await GetCountModAsync(playerId, mode, modHdHr, dateMin, dateMax),
			CountPlayedHDDT = await GetCountModAsync(playerId, mode, modHdDt, dateMin, dateMax)
		};
	}
	
	private async Task<int> GetAverageModScoreAsync(int playerId, int mode, int mods, DateTime dateMin, DateTime dateMax)
	{
		return await _scoresRepository.AverageModScoreAsync(playerId, mode, mods, dateMin, dateMax);
	}
	
	private async Task<int> GetCountModAsync(int playerId, int mode, int mods, DateTime dateMin, DateTime dateMax)
	{
		return await _scoresRepository.CountModScoresAsync(playerId, mode, mods, dateMin, dateMax);
	}
}