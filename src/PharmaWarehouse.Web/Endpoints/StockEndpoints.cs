using MediatR;
using PharmaWarehouse.Application.Core.Commands.Stock;

namespace PharmaWarehouse.Web.Endpoints;

public record AddStockRequest(int Quantity, string? Note);
public record WriteOffRequest(string? Note);

public static class StockEndpoints
{
    public static void MapStockEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/stock").WithTags("Stock").RequireAuthorization();

        group.MapPost("/dispatch", async (DispatchStockCommand cmd, ISender sender)
            => Results.Ok(await sender.Send(cmd)));

        group.MapPost("/batches/{batchId:int}/add", async (int batchId, AddStockRequest body, ISender sender) =>
        {
            var quantity = await sender.Send(new AddStockCommand(batchId, body.Quantity, body.Note));
            return Results.Ok(new { quantity });
        });

        group.MapPost("/batches/{batchId:int}/write-off", async (int batchId, WriteOffRequest body, ISender sender) =>
        {
            var writtenOff = await sender.Send(new WriteOffBatchCommand(batchId, body.Note));
            return Results.Ok(new { writtenOff });
        });
    }
}