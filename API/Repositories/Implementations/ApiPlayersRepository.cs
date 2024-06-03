using System.Diagnostics.CodeAnalysis;
using API.DTOs;
using API.Repositories.Interfaces;
using Database;
using Database.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class ApiPlayersRepository(OtrContext context) : PlayersRepository(context), IApiPlayersRepository
{
    private readonly OtrContext _context = context;

    public async Task<IEnumerable<PlayerIdMappingDTO>> GetIdMappingAsync() =>
        (await _context.Players.AsNoTracking().ToDictionaryAsync(p => p.OsuId, p => p.Id))
            .OrderBy(x => x.Value)
            .Select(x => new PlayerIdMappingDTO { Id = x.Value, OsuId = x.Key });

    public async Task<IEnumerable<PlayerCountryMappingDTO>> GetCountryMappingAsync() =>
        await _context
            .Players.AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new PlayerCountryMappingDTO { PlayerId = x.Id, Country = x.Country })
            .ToListAsync();
}
