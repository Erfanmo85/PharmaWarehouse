using PharmaWarehouse.Domain.Entities;

namespace PharmaWarehouse.Application.Repository;

public interface IBatchRepository : IRepository<Batch>
{
    Task<bool> NumberExistsAsync(int drugId, string batchNumber, CancellationToken ct);
    Task<List<Batch>> GetByDrugAsync(int drugId, CancellationToken ct);
}