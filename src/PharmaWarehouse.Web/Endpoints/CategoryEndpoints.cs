using MediatR;
using PharmaWarehouse.Application.Core.Commands.Categories;
using PharmaWarehouse.Application.Core.Queries.Categories;

namespace PharmaWarehouse.Web.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories").WithTags("Categories").RequireAuthorization();

        group.MapGet("/", async (ISender sender)
            => Results.Ok(await sender.Send(new GetCategoriesQuery())));

        group.MapPost("/", async (CreateCategoryCommand cmd, ISender sender) =>
        {
            var id = await sender.Send(cmd);
            return Results.Created("/api/categories", new { id });
        });
    }
}