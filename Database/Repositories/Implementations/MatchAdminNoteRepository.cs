using Database.Entities;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories.Implementations;

public class MatchAdminNoteRepository(OtrContext context) : AdminNoteRepositoryBase<MatchAdminNote>(context), IMatchAdminNoteRepository
{
    // CS 9107
    private readonly OtrContext _context = context;

    public override async Task<ICollection<MatchAdminNote>> GetAdminNotesAsync(int referenceId)
    {
        return await _context.MatchAdminNotes
            .Where(x => x.ReferenceId == referenceId)
            .ToListAsync();
    }
}
