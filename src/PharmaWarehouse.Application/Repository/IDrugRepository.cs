using PharmaWarehouse.Domain.Entities;

namespace PharmaWarehouse.Application.Repository;

public interface IDrugRepository : IRepository<Drug>
{
    Task<Drug?> GetByIdWithDetailsAsync(int id, CancellationToken ct);
    Task<bool> CodeExistsAsync(string code, int? excludeId, CancellationToken ct);
    Task<(List<Drug> Items, int Total)> SearchAsync(
        string? search, int page, int pageSize, CancellationToken ct);
}