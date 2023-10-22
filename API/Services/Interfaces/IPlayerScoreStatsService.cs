using API.DTOs;

namespace API.Services.Interfaces;

public interface IPlayerScoreStatsService
{
	Task<PlayerScoreStatsDTO> GetScoreStatsAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax);
}