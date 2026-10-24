using MediatR;
using PharmaWarehouse.Application.Common.Exceptions;
using PharmaWarehouse.Application.Repository;
using PharmaWarehouse.Domain.Entities;

namespace PharmaWarehouse.Application.Core.Queries.Batches;

public record BatchDto(
    int Id, string BatchNumber, DateOnly ManufactureDate,
    DateOnly ExpiryDate, int Quantity, decimal PurchasePrice, bool IsExpired);

public record GetBatchesByDrugQuery(int DrugId) : IRequest<List<BatchDto>>;

public class GetBatchesByDrugHandler : IRequestHandler<GetBatchesByDrugQuery, List<BatchDto>>
{
    private readonly IDrugRepository _drugs;
    private readonly IBatchRepository _batches;

    public GetBatchesByDrugHandler(IDrugRepository drugs, IBatchRepository batches)
    {
        _drugs = drugs;
        _batches = batches;
    }

    public async Task<List<BatchDto>> Handle(GetBatchesByDrugQuery request, CancellationToken ct)
    {
        if (await _drugs.GetByIdAsync(request.DrugId, ct) is null)
            throw new NotFoundException(nameof(Drug), request.DrugId);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var batches = await _batches.GetByDrugAsync(request.DrugId, ct);

        return batches
            .Select(b => new BatchDto(b.Id, b.BatchNumber, b.ManufactureDate,
                b.ExpiryDate, b.Quantity, b.PurchasePrice, b.IsExpired(today)))
            .ToList();
    }
}