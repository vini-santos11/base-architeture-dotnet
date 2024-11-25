using Application.Models;

namespace API.Middlewares;

public class ForbiddenMiddleware
{
    private readonly RequestDelegate _next;

    public ForbiddenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
        {
            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var response = new Response<object>()
            {
                Success = false,
                Message = "Acesso negado. Você não tem permissão para acessar este recurso."
            };
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}