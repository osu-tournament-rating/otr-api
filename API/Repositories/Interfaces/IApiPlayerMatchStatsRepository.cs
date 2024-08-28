using API.DTOs;
using Database.Enums;
using Database.Repositories.Interfaces;

namespace API.Repositories.Interfaces;

public interface IApiPlayerMatchStatsRepository : IPlayerMatchStatsRepository
{
    Task<PlayerModStatsDTO> GetModStatsAsync(int playerId, Ruleset ruleset, DateTime dateMin, DateTime dateMax);
}
