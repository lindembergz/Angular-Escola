using Microsoft.EntityFrameworkCore;
using SistemaGestaoEscolar.Shared.Domain.Entities;
using SistemaGestaoEscolar.Shared.Domain.Interfaces;

namespace SistemaGestaoEscolar.Shared.Infrastructure.Repositories;

public abstract class BaseRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly DbContext Context;
    protected readonly DbSet<T> DbSet;

    protected BaseRepository(DbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await DbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }

    public virtual async Task AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
    }

    public virtual Task UpdateAsync(T entity)
    {
        entity.MarkAsUpdated();
        DbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(T entity)
    {
        DbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual async Task<bool> ExistsAsync(Guid id)
    {
        return await DbSet.AnyAsync(e => e.Id == id);
    }

    public virtual async Task<int> CountAsync()
    {
        return await DbSet.CountAsync();
    }

    public virtual async Task SaveChangesAsync()
    {
        await Context.SaveChangesAsync();
    }

    protected virtual IQueryable<T> GetQueryable()
    {
        return DbSet.AsQueryable();
    }

    protected virtual IQueryable<T> GetQueryableWithTracking()
    {
        return DbSet.AsTracking();
    }

    protected virtual IQueryable<T> GetQueryableNoTracking()
    {
        return DbSet.AsNoTracking();
    }
}