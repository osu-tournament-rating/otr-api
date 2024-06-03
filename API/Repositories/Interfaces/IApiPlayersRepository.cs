using API.DTOs;
using Database.Repositories.Interfaces;

namespace API.Repositories.Interfaces;

public interface IApiPlayersRepository : IPlayersRepository
{
    /// <summary>
    /// Returns a collection of <see cref="PlayerIdMappingDTO"/> for all players
    /// </summary>
    Task<IEnumerable<PlayerIdMappingDTO>> GetIdMappingAsync();

    /// <summary>
    /// Returns a collection of <see cref="PlayerCountryMappingDTO"/> for all players
    /// </summary>
    Task<IEnumerable<PlayerCountryMappingDTO>> GetCountryMappingAsync();
}
