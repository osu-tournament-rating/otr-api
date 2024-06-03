using API.DTOs;
using Database.Repositories.Interfaces;

namespace API.Repositories.Interfaces;

public interface IApiPlayerMatchStatsRepository : IPlayerMatchStatsRepository
{
    Task<PlayerModStatsDTO> GetModStatsAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax);
}
