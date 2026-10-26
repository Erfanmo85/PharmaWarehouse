using MediatR;
using PharmaWarehouse.Application.Core.Queries.Reports;

namespace PharmaWarehouse.Web.Endpoints;

public static class ReportEndpoints
{
    public static void MapReportEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reports").WithTags("Reports").RequireAuthorization();

        group.MapGet("/low-stock", async (ISender sender)
            => Results.Ok(await sender.Send(new GetLowStockQuery())));

        group.MapGet("/expiring", async (ISender sender, int days = 30)
            => Results.Ok(await sender.Send(new GetExpiringBatchesQuery(days))));
    }
}