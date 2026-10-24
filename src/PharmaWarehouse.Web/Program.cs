using System.Text.Json.Serialization;
using PharmaWarehouse.Application;
using PharmaWarehouse.Infrastructure;
using PharmaWarehouse.Web.Endpoints;
using PharmaWarehouse.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// لایه‌ها
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// مدیریت خطا
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Enum را هم به صورت متن بپذیر و برگردان (مثلاً "Tablet")
builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// اجازه به Angular
builder.Services.AddCors(o => o.AddPolicy("Angular", p =>
    p.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod()));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Angular");

app.MapGet("/", () => "Pharma Warehouse API is running");
app.MapCategoryEndpoints();
app.MapDrugEndpoints();
app.MapBatchEndpoints();

app.Run();