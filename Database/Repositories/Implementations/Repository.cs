﻿using Database.Entities.Interfaces;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Database.Repositories.Implementations;

/// <summary>
/// Base implementation of <see cref="IRepository{T}"/> providing common CRUD operations
/// </summary>
public class Repository<T> : IRepository<T> where T : class, IEntity
{
    private readonly OtrContext _context;

    public LocalView<T> LocalView => _context.Set<T>().Local;

    protected Repository(OtrContext context)
    {
        _context = context;
    }

    public virtual void Add(T entity) =>
        _context.Set<T>().Add(entity);

    public virtual void AddRange(IEnumerable<T> entities) =>
        _context.Set<T>().AddRange(entities);

    public virtual async Task<T> CreateAsync(T entity)
    {
        T created = (await _context.Set<T>().AddAsync(entity)).Entity;
        await _context.SaveChangesAsync();

        return created;
    }

    public virtual async Task<T?> GetAsync(int id) => await _context.Set<T>().FindAsync(id);

    public virtual async Task<int> UpdateAsync(T entity)
    {
        if (entity is IUpdateableEntity updateableEntity)
        {
            updateableEntity.Updated = DateTime.UtcNow;
            _context.Set<T>().Update((T)updateableEntity);
        }
        else
        {
            _context.Set<T>().Update(entity);
        }

        return await _context.SaveChangesAsync();
    }

    public virtual async Task<int> UpdateAsync(IEnumerable<T> entities)
    {
        foreach (T entity in entities)
        {
            if (entity is IUpdateableEntity updateableEntity)
            {
                updateableEntity.Updated = DateTime.UtcNow;
                _context.Set<T>().Update((T)updateableEntity);
            }
            else
            {
                _context.Set<T>().Update(entity);
            }
        }

        return await _context.SaveChangesAsync();
    }

    public TUpdateable MarkUpdated<TUpdateable>(TUpdateable entity) where TUpdateable : IUpdateableEntity
    {
        entity.Updated = DateTime.UtcNow;
        return entity;
    }

    public virtual async Task<int?> DeleteAsync(int id)
    {
        T? entity = await _context.Set<T>().FindAsync(id);
        if (entity == null)
        {
            return null;
        }

        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
        return id;
    }

    public virtual async Task<bool> ExistsAsync(int id) => await _context.Set<T>().FindAsync(id) != null;

    public virtual async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().AsNoTracking().ToListAsync();
}
