using MediatR;
using PharmaWarehouse.Application.Common.Exceptions;
using PharmaWarehouse.Application.Repository;
using PharmaWarehouse.Domain.Entities;

namespace PharmaWarehouse.Application.Core.Queries.Drugs;

public record GetDrugByIdQuery(int Id) : IRequest<DrugDto>;

public class GetDrugByIdHandler : IRequestHandler<GetDrugByIdQuery, DrugDto>
{
    private readonly IDrugRepository _drugs;

    public GetDrugByIdHandler(IDrugRepository drugs)
    {
        _drugs = drugs;
    }

    public async Task<DrugDto> Handle(GetDrugByIdQuery request, CancellationToken ct)
    {
        var drug = await _drugs.GetByIdWithDetailsAsync(request.Id, ct)
                   ?? throw new NotFoundException(nameof(Drug), request.Id);

        return DrugDto.FromEntity(drug);
    }
}