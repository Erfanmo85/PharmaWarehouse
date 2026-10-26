using PharmaWarehouse.Web.Auth;

namespace PharmaWarehouse.Web.Endpoints;

public record LoginRequest(string Username, string Password);

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", (LoginRequest body, IConfiguration config, TokenService tokens) =>
        {
            var username = config["AdminUser:Username"];
            var password = config["AdminUser:Password"];

            if (body.Username != username || body.Password != password)
                return Results.Unauthorized();

            return Results.Ok(new { token = tokens.CreateToken(body.Username) });
        }).WithTags("Auth");
    }
}