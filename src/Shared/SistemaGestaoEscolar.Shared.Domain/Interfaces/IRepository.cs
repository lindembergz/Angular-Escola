using SistemaGestaoEscolar.Shared.Domain.Entities;

namespace SistemaGestaoEscolar.Shared.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<bool> ExistsAsync(Guid id);
    Task<int> CountAsync();
    Task SaveChangesAsync();
}