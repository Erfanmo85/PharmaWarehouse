using MediatR;
using PharmaWarehouse.Application.Core.Commands.Drugs;
using PharmaWarehouse.Application.Core.Queries.Drugs;
using PharmaWarehouse.Domain.Enums;

namespace PharmaWarehouse.Web.Endpoints;

public record UpdateDrugRequest(
    string Name, string Code, DosageForm DosageForm, int MinStockLevel, int CategoryId);

public static class DrugEndpoints
{
    public static void MapDrugEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/drugs").WithTags("Drugs").RequireAuthorization();

        group.MapGet("/", async (ISender sender, string? search, int page = 1, int pageSize = 10)
            => Results.Ok(await sender.Send(new GetDrugsQuery(search, page, pageSize))));

        group.MapGet("/{id:int}", async (int id, ISender sender)
            => Results.Ok(await sender.Send(new GetDrugByIdQuery(id))));

        group.MapPost("/", async (CreateDrugCommand cmd, ISender sender) =>
        {
            var id = await sender.Send(cmd);
            return Results.Created($"/api/drugs/{id}", new { id });
        });

        group.MapPut("/{id:int}", async (int id, UpdateDrugRequest body, ISender sender) =>
        {
            await sender.Send(new UpdateDrugCommand(
                id, body.Name, body.Code, body.DosageForm, body.MinStockLevel, body.CategoryId));
            return Results.NoContent();
        });

        group.MapDelete("/{id:int}", async (int id, ISender sender) =>
        {
            await sender.Send(new DeleteDrugCommand(id));
            return Results.NoContent();
        });
    }
}