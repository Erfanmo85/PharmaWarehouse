using MediatR;
using PharmaWarehouse.Application;
using PharmaWarehouse.Application.Core.Commands.Categories;
using PharmaWarehouse.Application.Core.Queries.Categories;
using PharmaWarehouse.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "Pharma Warehouse API is running");

// ---- تست موقت (فردا حذف می‌شود) ----
app.MapPost("/test/category", async (CreateCategoryCommand cmd, ISender sender)
    => Results.Ok(await sender.Send(cmd)));

app.MapGet("/test/categories", async (ISender sender)
    => Results.Ok(await sender.Send(new GetCategoriesQuery())));

app.Run();