using PharmaWarehouse.Domain.Entities;

namespace PharmaWarehouse.Application.Repository;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    void Remove(T entity);
}