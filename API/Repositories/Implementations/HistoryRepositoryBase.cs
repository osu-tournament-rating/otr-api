using System.Diagnostics.CodeAnalysis;
using API.Entities.Interfaces;
using API.Enums;
using API.Repositories.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class HistoryRepositoryBase<TEntity, THistory>
    : RepositoryBase<TEntity>,
        IHistoryRepository<TEntity, THistory>
    where THistory : class, IHistoryEntity
    where TEntity : class, IEntity
{
    private readonly OtrContext _context;
    private readonly IMapper _mapper;

    protected HistoryRepositoryBase(OtrContext context, IMapper mapper)
        : base(context)
    {
        _mapper = mapper;
        _context = context;
    }

    public override async Task<int> UpdateAsync(TEntity entity)
    {
        await CreateHistoryAsync(entity.Id, HistoryActionType.Update);
        if (entity is IUpdateableEntity updateableEntity)
        {
            updateableEntity.Updated = DateTime.UtcNow;
        }
        return await base.UpdateAsync(entity);
    }

    public override async Task<int?> DeleteAsync(int id)
    {
        await CreateHistoryAsync(id, HistoryActionType.Delete);
        return await base.DeleteAsync(id);
    }

    public async Task<int> UpdateAsync(TEntity entity, int modifierId)
    {
        await CreateHistoryAsync(entity.Id, HistoryActionType.Update, modifierId);
        if (entity is IUpdateableEntity updateableEntity)
        {
            updateableEntity.Updated = DateTime.UtcNow;
        }
        return await base.UpdateAsync(entity);
    }

    public async Task<int?> DeleteAsync(int id, int modifierId)
    {
        await CreateHistoryAsync(id, HistoryActionType.Delete, modifierId);
        return await base.DeleteAsync(id);
    }

    private async Task<TEntity?> GetNoTrackingAsync(int id) =>
        await _context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

    private async Task<THistory?> CreateHistoryAsync(int id, HistoryActionType action, int? modifierId = null)
    {
        TEntity? origEntity = await GetNoTrackingAsync(id);
        if (origEntity == null)
        {
            return null;
        }

        THistory record = _mapper.Map<THistory>(origEntity);
        record.HistoryAction = (int)action;
        // Modifications made automatically (without user intervention) have an Id of null
        record.ModifierId = modifierId;

        THistory created = (await _context.Set<THistory>().AddAsync(record)).Entity;
        await _context.SaveChangesAsync();

        return created;
    }
}
