using System.Diagnostics.CodeAnalysis;
using API.Repositories.Interfaces;
using AutoMapper;
using Database;
using Database.Entities.Interfaces;
using Database.Enums;
using Database.Repositories.Implementations;
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
        return await UpdateAsync(entity, null);
    }

    public override async Task<int> UpdateAsync(IEnumerable<TEntity> entities)
    {
        return await UpdateAsync(entities, null);
    }

    public override async Task<int?> DeleteAsync(int id)
    {
        return await DeleteAsync(id, null);
    }

    public async Task<int> UpdateAsync(TEntity entity, int? modifierId)
    {
        await CreateHistoryAsync(entity.Id, HistoryActionType.Update, modifierId);
        return await base.UpdateAsync(entity);
    }

    public async Task<int> UpdateAsync(IEnumerable<TEntity> entities, int? modifierId)
    {
        IEnumerable<TEntity> enumerable = entities as TEntity[] ?? entities.ToArray();
        await CreateHistoryAsync(enumerable.Select(x => x.Id), HistoryActionType.Update, modifierId);

        return await base.UpdateAsync(enumerable);
    }

    public async Task<int?> DeleteAsync(int id, int? modifierId)
    {
        await CreateHistoryAsync(id, HistoryActionType.Delete, modifierId);
        return await base.DeleteAsync(id);
    }

    private async Task<TEntity?> GetNoTrackingAsync(int id) =>
        await _context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

    private async Task<IEnumerable<TEntity>> GetNoTrackingAsync(IEnumerable<int> ids) =>
        await _context.Set<TEntity>().AsNoTracking().Where(x => ids.Contains(x.Id)).ToListAsync();

    private async Task CreateHistoryAsync(int id, HistoryActionType action, int? modifierId = null)
    {
        TEntity? origEntity = await GetNoTrackingAsync(id);
        if (origEntity == null)
        {
            return;
        }

        THistory record = _mapper.Map<THistory>(origEntity);
        record.HistoryAction = (int)action;
        // Modifications made automatically (without user intervention) have an Id of null
        record.ModifierId = modifierId;

        await _context.Set<THistory>().AddAsync(record);
        await _context.SaveChangesAsync();
    }

    private async Task CreateHistoryAsync(IEnumerable<int> ids, HistoryActionType action,
        int? modifierId = null)
    {
        foreach (TEntity origEntity in await GetNoTrackingAsync(ids))
        {
            THistory record = _mapper.Map<THistory>(origEntity);
            record.HistoryAction = (int)action;
            // Modifications made automatically (without user intervention) have an Id of null
            record.ModifierId = modifierId;
            await _context.Set<THistory>().AddAsync(record);
        }
        await _context.SaveChangesAsync();
    }
}
