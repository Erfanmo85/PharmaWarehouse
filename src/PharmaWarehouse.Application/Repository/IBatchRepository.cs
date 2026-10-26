using PharmaWarehouse.Domain.Entities;

namespace PharmaWarehouse.Application.Repository;

public interface IBatchRepository : IRepository<Batch>
{
    Task<bool> NumberExistsAsync(int drugId, string batchNumber, CancellationToken ct);
    Task<List<Batch>> GetByDrugAsync(int drugId, CancellationToken ct);

    // بچ‌های قابل عرضه یک دارو، مرتب‌شده بر اساس نزدیک‌ترین انقضا (FEFO)
    Task<List<Batch>> GetAvailableForDispatchAsync(int drugId, DateOnly today, CancellationToken ct);

    // بچ‌هایی که تا تاریخ مشخص منقضی می‌شوند (یا شده‌اند) و هنوز موجودی دارند
    Task<List<Batch>> GetExpiringAsync(DateOnly until, CancellationToken ct);
}