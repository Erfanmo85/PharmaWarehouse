using Microsoft.EntityFrameworkCore;
using PharmaWarehouse.Application.Repository;
using PharmaWarehouse.Domain.Entities;
using PharmaWarehouse.Infrastructure.Persistence;

namespace PharmaWarehouse.Infrastructure.Repositories;

public class BaseRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext Db;

    public BaseRepository(AppDbContext db)
    {
        Db = db;
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        => await Db.Set<T>().FindAsync(new object[] { id }, ct);

    public async Task AddAsync(T entity, CancellationToken ct = default)
        => await Db.Set<T>().AddAsync(entity, ct);

    public void Remove(T entity)
        => Db.Set<T>().Remove(entity);
}