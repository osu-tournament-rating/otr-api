using API.Entities;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class MatchDuplicateRepository(OtrContext context) : RepositoryBase<MatchDuplicate>(context), IMatchDuplicateRepository
{
    private readonly OtrContext _context = context;

    public async Task<IEnumerable<MatchDuplicate>> GetDuplicatesAsync(int matchId) =>
        await _context.MatchDuplicates.Where(x => x.SuspectedDuplicateOf == matchId).ToListAsync();

    public async Task<IEnumerable<MatchDuplicate>> GetAllUnknownStatusAsync() =>
        await _context.MatchDuplicates.Where(x => x.VerifiedAsDuplicate != true).ToListAsync();

    public override async Task<IEnumerable<MatchDuplicate>> GetAllAsync() =>
        await _context.MatchDuplicates.Include(x => x.Verifier).ThenInclude(x => x!.Player).ToListAsync();
}
