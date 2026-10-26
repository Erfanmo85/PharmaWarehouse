using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PharmaWarehouse.Application.Common.Exceptions;

namespace PharmaWarehouse.Web.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context, Exception exception, CancellationToken ct)
    {
        ProblemDetails problem;

        switch (exception)
        {
            case ValidationException ve:
                {
                    var errors = ve.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                    problem = new ValidationProblemDetails(errors)
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "داده‌های ارسالی نامعتبر است."
                    };
                    break;
                }
            case NotFoundException:
                problem = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = exception.Message
                };
                break;
            case ConflictException:
                problem = new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = exception.Message
                };
                break;
            default:
                _logger.LogError(exception, "خطای مدیریت‌نشده");
                problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "خطای داخلی سرور.",
                    Detail = _env.IsDevelopment()
                        ? $"{exception.GetType().Name}: {exception.Message} | {exception.InnerException?.Message}"
                        : null
                };
                break;
        }

        context.Response.StatusCode = problem.Status!.Value;
        await context.Response.WriteAsJsonAsync(problem, ct);
        return true;
    }
}