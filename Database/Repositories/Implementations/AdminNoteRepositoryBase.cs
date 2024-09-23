using Database.Entities;
using Database.Repositories.Interfaces;

namespace Database.Repositories.Implementations;

public abstract class AdminNoteRepositoryBase<TAdminNoteEntity>(OtrContext context)
    : RepositoryBase<TAdminNoteEntity>(context), IAdminNoteRepository<TAdminNoteEntity>
    where TAdminNoteEntity : AdminNoteEntityBase
{
    public abstract Task<ICollection<TAdminNoteEntity>> GetAdminNotesAsync(int referenceId);
}
