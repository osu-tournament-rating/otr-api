using Database.Entities;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Database.Repositories.Implementations;

public class AdminNoteRepository(OtrContext context) : IAdminNoteRepository
{
    public async Task CreateAsync<TAdminNote>(TAdminNote entity) where TAdminNote : AdminNoteEntityBase
    {
        context.Set<TAdminNote>().Add(entity);
        await context.SaveChangesAsync();
    }

    public async Task<TAdminNote?> GetAsync<TAdminNote>(int id) where TAdminNote : AdminNoteEntityBase =>
        await context.Set<TAdminNote>()
            .Include(an => an.AdminUser.Player)
            .AsSingleQuery()
            .FirstOrDefaultAsync(an => an.Id == id);

    public async Task<IEnumerable<TAdminNote>> ListAsync<TAdminNote>(int referenceId) where TAdminNote : AdminNoteEntityBase =>
        await context.Set<TAdminNote>()
            .Where(an => an.ReferenceId == referenceId)
            .Include(an => an.AdminUser.Player)
            .AsNoTracking()
            .AsSingleQuery()
            .ToListAsync();

    public async Task<TAdminNote> UpdateAsync<TAdminNote>(TAdminNote entity) where TAdminNote : AdminNoteEntityBase
    {
        EntityEntry<TAdminNote> entry = context.Set<TAdminNote>().Entry(entity);

        if (entry.State is EntityState.Unchanged)
        {
            return entity;
        }

        await context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync<TAdminNote>(TAdminNote entity) where TAdminNote : AdminNoteEntityBase
    {
        context.Set<TAdminNote>().Remove(entity);
        await context.SaveChangesAsync();
    }
}
