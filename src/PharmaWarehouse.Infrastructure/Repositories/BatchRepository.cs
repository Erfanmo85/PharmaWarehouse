using Microsoft.EntityFrameworkCore;
using PharmaWarehouse.Application.Repository;
using PharmaWarehouse.Domain.Entities;
using PharmaWarehouse.Infrastructure.Persistence;

namespace PharmaWarehouse.Infrastructure.Repositories;

public class BatchRepository : BaseRepository<Batch>, IBatchRepository
{
    public BatchRepository(AppDbContext db) : base(db) { }

    public Task<bool> NumberExistsAsync(int drugId, string batchNumber, CancellationToken ct)
        => Db.Batches.AnyAsync(b => b.DrugId == drugId && b.BatchNumber == batchNumber, ct);

    public Task<List<Batch>> GetByDrugAsync(int drugId, CancellationToken ct)
        => Db.Batches
            .AsNoTracking()
            .Where(b => b.DrugId == drugId)
            .OrderBy(b => b.ExpiryDate)
            .ToListAsync(ct);


    public Task<List<Batch>> GetAvailableForDispatchAsync(int drugId, DateOnly today, CancellationToken ct)
        => Db.Batches
            .Where(b => b.DrugId == drugId && b.Quantity > 0 && b.ExpiryDate >= today)
            .OrderBy(b => b.ExpiryDate)
            .ToListAsync(ct);

    public Task<List<Batch>> GetExpiringAsync(DateOnly until, CancellationToken ct)
        => Db.Batches
            .AsNoTracking()
            .Include(b => b.Drug)
            .Where(b => b.Quantity > 0 && b.ExpiryDate <= until)
            .OrderBy(b => b.ExpiryDate)
            .ToListAsync(ct);
};