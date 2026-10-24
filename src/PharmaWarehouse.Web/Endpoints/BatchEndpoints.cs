using MediatR;
using PharmaWarehouse.Application.Core.Commands.Batches;
using PharmaWarehouse.Application.Core.Queries.Batches;

namespace PharmaWarehouse.Web.Endpoints;

public static class BatchEndpoints
{
    public static void MapBatchEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/batches", async (CreateBatchCommand cmd, ISender sender) =>
        {
            var id = await sender.Send(cmd);
            return Results.Created($"/api/drugs/{cmd.DrugId}/batches", new { id });
        }).WithTags("Batches");

        app.MapGet("/api/drugs/{drugId:int}/batches", async (int drugId, ISender sender)
            => Results.Ok(await sender.Send(new GetBatchesByDrugQuery(drugId))))
            .WithTags("Batches");
    }
}