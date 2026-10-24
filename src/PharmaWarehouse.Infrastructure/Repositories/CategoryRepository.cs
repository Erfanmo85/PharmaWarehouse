using Microsoft.EntityFrameworkCore;
using PharmaWarehouse.Application.Repository;
using PharmaWarehouse.Domain.Entities;
using PharmaWarehouse.Infrastructure.Persistence;

namespace PharmaWarehouse.Infrastructure.Repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext db) : base(db) { }

    public Task<List<Category>> GetAllAsync(CancellationToken ct)
        => Db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(ct);

    public Task<bool> ExistsAsync(int id, CancellationToken ct)
        => Db.Categories.AnyAsync(c => c.Id == id, ct);

    public Task<bool> NameExistsAsync(string name, CancellationToken ct)
        => Db.Categories.AnyAsync(c => c.Name == name, ct);
}