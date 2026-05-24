using ElectroShop.Application.Common.Results;
using ElectroShop.Domain.Exceptions;
using ElectroShop.Persistence.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace ElectroShop.WebApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var result = Result.Failure(Error.Failure(
            "General.ServerError",
            "Server xətası baş verdi. Zəhmət olmasa yenidən cəhd edin."));

        if (exception is ArgumentException argEx)
        {
            code = HttpStatusCode.BadRequest;
            result = Result.Failure(Error.Validation(
                "Validation.ArgumentError",
                argEx.Message));
        }
        else if (exception is InvalidOperationException invalidOpEx)
        {
            code = HttpStatusCode.BadRequest;
            result = Result.Failure(Error.Validation(
                "Validation.InvalidOperation",
                invalidOpEx.Message));
        }
        else if (exception is UnauthorizedAccessException)
        {
            code = HttpStatusCode.Unauthorized;
            result = Result.Failure(Error.Unauthorized(
                "Authentication.Unauthorized",
                "Bu əməliyyat üçün icazəniz yoxdur."));
        }
        else if (exception is ConcurrencyException or DbUpdateConcurrencyException)
        {
            code = HttpStatusCode.Conflict;
            result = Result.Failure(Error.Conflict(
                "Entity.ConcurrencyConflict",
                "Məlumat başqa istifadəçi tərəfindən dəyişdirilib. Zəhmət olmasa yenidən yükləyin."));
        }
        else if (exception is DbUpdateException dbUpdateEx
                 && DatabaseExceptionMapper.TryMap(dbUpdateEx) is { } mappedError)
        {
            code = HttpStatusCode.Conflict;
            result = Result.Failure(mappedError);
        }

        var response = context.Response;
        response.ContentType = "application/json";
        response.StatusCode = (int)code;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return response.WriteAsync(JsonSerializer.Serialize(result, options));
    }
}

