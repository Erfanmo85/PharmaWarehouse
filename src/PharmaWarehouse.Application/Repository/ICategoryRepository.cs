using PharmaWarehouse.Domain.Entities;

namespace PharmaWarehouse.Application.Repository;

public interface ICategoryRepository : IRepository<Category>
{
    Task<List<Category>> GetAllAsync(CancellationToken ct);
    Task<bool> ExistsAsync(int id, CancellationToken ct);
    Task<bool> NameExistsAsync(string name, CancellationToken ct);
}