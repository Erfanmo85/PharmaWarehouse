using FluentValidation;
using MediatR;
using PharmaWarehouse.Application.Common.Models;
using PharmaWarehouse.Application.Repository;

namespace PharmaWarehouse.Application.Core.Queries.Drugs;

public record GetDrugsQuery(string? Search, int Page = 1, int PageSize = 10)
    : IRequest<PagedResult<DrugDto>>;

public class GetDrugsValidator : AbstractValidator<GetDrugsQuery>
{
    public GetDrugsValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public class GetDrugsHandler : IRequestHandler<GetDrugsQuery, PagedResult<DrugDto>>
{
    private readonly IDrugRepository _drugs;

    public GetDrugsHandler(IDrugRepository drugs)
    {
        _drugs = drugs;
    }

    public async Task<PagedResult<DrugDto>> Handle(GetDrugsQuery request, CancellationToken ct)
    {
        var (items, total) = await _drugs.SearchAsync(
            request.Search, request.Page, request.PageSize, ct);

        var dtos = items.Select(DrugDto.FromEntity).ToList();

        return new PagedResult<DrugDto>(dtos, total, request.Page, request.PageSize);
    }
}