using MediatR;
using PharmaWarehouse.Application.Repository;

namespace PharmaWarehouse.Application.Core.Queries.Categories;

public record CategoryDto(int Id, string Name);

public record GetCategoriesQuery : IRequest<List<CategoryDto>>;

public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    private readonly ICategoryRepository _categories;

    public GetCategoriesHandler(ICategoryRepository categories)
    {
        _categories = categories;
    }

    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken ct)
    {
        var items = await _categories.GetAllAsync(ct);
        return items.Select(c => new CategoryDto(c.Id, c.Name)).ToList();
    }
}