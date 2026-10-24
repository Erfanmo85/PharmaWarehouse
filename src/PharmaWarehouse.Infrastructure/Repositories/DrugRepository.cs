using Microsoft.EntityFrameworkCore;
using PharmaWarehouse.Application.Repository;
using PharmaWarehouse.Domain.Entities;
using PharmaWarehouse.Infrastructure.Persistence;

namespace PharmaWarehouse.Infrastructure.Repositories;

public class DrugRepository : BaseRepository<Drug>, IDrugRepository
{
    public DrugRepository(AppDbContext db) : base(db) { }

    public Task<Drug?> GetByIdWithDetailsAsync(int id, CancellationToken ct)
        => Db.Drugs
            .AsNoTracking()
            .Include(d => d.Category)
            .Include(d => d.Batches)
            .FirstOrDefaultAsync(d => d.Id == id, ct);

    public Task<bool> CodeExistsAsync(string code, int? excludeId, CancellationToken ct)
        => Db.Drugs.AnyAsync(d => d.Code == code && (excludeId == null || d.Id != excludeId), ct);

    public async Task<(List<Drug> Items, int Total)> SearchAsync(
        string? search, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Drug> query = Db.Drugs
            .AsNoTracking()
            .Include(d => d.Category)
            .Include(d => d.Batches);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(d => d.Name.Contains(search) || d.Code.Contains(search));

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(d => d.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }
}