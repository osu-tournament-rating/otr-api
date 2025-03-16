using Database.Entities;
using Database.Entities.Interfaces;
using Database.Repositories.Interfaces;
using Database.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Database.Repositories.Implementations;

public class AdminNoteRepository(OtrContext context) : IAdminNoteRepository
{
    public async Task<bool> ExistsAsync<TAdminNote>(int id) where TAdminNote : AdminNoteEntityBase =>
        await context.Set<TAdminNote>().AnyAsync(an => an.Id == id);

    public async Task<bool> ExistsAsync(Type adminNoteType, int id) =>
        await DbSet(adminNoteType).AnyAsync(an => an.Id == id);

    public async Task<TAdminNote> CreateAsync<TAdminNote>(TAdminNote entity) where TAdminNote : AdminNoteEntityBase
    {
        context.Set<TAdminNote>().Add(entity);
        await context.SaveChangesAsync();

        return entity;
    }

    public async Task<IAdminNoteEntity> CreateAsync(IAdminNoteEntity entity)
    {
        AssertEntityType(entity.GetType());

        context.Add(entity);
        await context.SaveChangesAsync();

        return entity;
    }

    public async Task<TAdminNote?> GetAsync<TAdminNote>(int id) where TAdminNote : AdminNoteEntityBase =>
        await context.Set<TAdminNote>()
            .AsSingleQuery()
            .Include(an => an.AdminUser.Player)
            .FirstOrDefaultAsync(an => an.Id == id);

    public async Task<IAdminNoteEntity?> GetAsync(Type adminNoteType, int id) =>
        await DbSet(adminNoteType)
            .AsSingleQuery()
            .Include(an => an.AdminUser.Player)
            .FirstOrDefaultAsync(an => an.Id == id);

    public async Task<IEnumerable<TAdminNote>> ListAsync<TAdminNote>(int referenceId) where TAdminNote : AdminNoteEntityBase =>
        await context.Set<TAdminNote>()
            .AsNoTracking()
            .AsSingleQuery()
            .Include(an => an.AdminUser.Player)
            .Where(an => an.ReferenceId == referenceId)
            .ToListAsync();

    public async Task<IEnumerable<IAdminNoteEntity>> ListAsync(Type adminNoteType, int referenceId) =>
        await DbSet(adminNoteType)
            .AsNoTracking()
            .AsSingleQuery()
            .Include(an => an.AdminUser.Player)
            .Where(an => an.ReferenceId == referenceId)
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

    public async Task<IAdminNoteEntity> UpdateAsync(IAdminNoteEntity entity)
    {
        EntityEntry<IAdminNoteEntity> entry = context.Entry(entity);

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

    public async Task DeleteAsync(IAdminNoteEntity entity)
    {
        AssertEntityType(entity.GetType());

        context.Remove(entity);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Asserts the given type inherits from <see cref="AdminNoteEntityBase"/>
    /// </summary>
    /// <param name="entityType">Admin note entity type</param>
    private static void AssertEntityType(Type entityType)
    {
        if (!typeof(AdminNoteEntityBase).IsAssignableFrom(entityType))
        {
            throw new ArgumentException($"Type must implement {nameof(AdminNoteEntityBase)}", nameof(entityType));
        }
    }

    private IQueryable<IAdminNoteEntity> DbSet(Type entityType)
    {
        AssertEntityType(entityType);

        IQueryable<object>? dbSet = context.Set(entityType) ?? throw new Exception($"Could not resolve DbSet for admin note type {entityType.Name}");
        return (IQueryable<IAdminNoteEntity>)dbSet;
    }
}
