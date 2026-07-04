using System.Text.Json;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Contracts.Responses;

namespace TaskManager.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex.StatusCode;

            var response = new ApiErrorResponse
            {
                Action = ResolveAction(context),
                HttpStatusCode = ex.StatusCode,
                ErrorCode = ex.ErrorCode,
                Message = ex.Message,
                Path = context.Request.Path
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
        catch (Exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = new ApiErrorResponse
            {
                Action = ResolveAction(context),
                HttpStatusCode = StatusCodes.Status500InternalServerError,
                ErrorCode = "internal_server_error",
                Message = "An unexpected error occurred.",
                Path = context.Request.Path
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }

    private static string ResolveAction(HttpContext context)
    {
        return context.Request.Method.ToLowerInvariant() switch
        {
            "post" => "post",
            "put" => "put",
            "delete" => "delete",
            "get" => "get",
            _ => "request"
        };
    }

}