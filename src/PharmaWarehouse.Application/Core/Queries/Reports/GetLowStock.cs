using MediatR;
using PharmaWarehouse.Application.Repository;

namespace PharmaWarehouse.Application.Core.Queries.Reports;

public record LowStockDto(int DrugId, string Name, string Code, int TotalStock, int MinStockLevel);

public record GetLowStockQuery : IRequest<List<LowStockDto>>;

public class GetLowStockHandler : IRequestHandler<GetLowStockQuery, List<LowStockDto>>
{
    private readonly IDrugRepository _drugs;

    public GetLowStockHandler(IDrugRepository drugs)
    {
        _drugs = drugs;
    }

    public async Task<List<LowStockDto>> Handle(GetLowStockQuery request, CancellationToken ct)
    {
        var drugs = await _drugs.GetLowStockAsync(ct);

        return drugs
            .Select(d => new LowStockDto(d.Id, d.Name, d.Code, d.TotalStock, d.MinStockLevel))
            .ToList();
    }
}