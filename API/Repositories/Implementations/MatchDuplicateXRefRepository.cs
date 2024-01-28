using API.Entities;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class MatchDuplicateXRefRepository : RepositoryBase<MatchDuplicateXRef>, IMatchDuplicateXRefRepository
{
	private readonly OtrContext _context;
	public MatchDuplicateXRefRepository(OtrContext context) : base(context) { _context = context; }

	public async Task<IEnumerable<MatchDuplicateXRef>> GetDuplicatesAsync(int matchId) =>
		await _context.MatchDuplicates.Where(x => x.SuspectedDuplicateOf == matchId).ToListAsync();

	public override async Task<IEnumerable<MatchDuplicateXRef>> GetAllAsync() => await _context.MatchDuplicates.Include(x => x.Verifier).ThenInclude(x => x.Player).ToListAsync();
}