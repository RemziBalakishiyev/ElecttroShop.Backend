using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using ElectroShop.Application.Logging;
using Serilog.Context;

namespace ElectroShop.WebApi.Middleware;

public class RequestLoggingMiddleware
{
    private const int MaxRequestBodyLength = 8000;

    private static readonly HashSet<string> ExcludedPaths = new(StringComparer.OrdinalIgnoreCase)
    {
        "/health",
        "/swagger",
        "/favicon.ico"
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (ShouldSkip(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(correlationId))
            correlationId = Guid.NewGuid().ToString("N");

        context.Response.Headers["X-Correlation-Id"] = correlationId;
        context.Items["CorrelationId"] = correlationId;

        var stopwatch = Stopwatch.StartNew();
        var requestBody = await ReadRequestBodyAsync(context.Request);
        var clientIp = GetClientIp(context);
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userEmail = context.User.FindFirstValue(ClaimTypes.Email)
            ?? context.User.FindFirstValue(ClaimTypes.Name);

        using (LogContext.PushProperty(LogContextPropertyNames.CorrelationId, correlationId))
        using (LogContext.PushProperty(LogContextPropertyNames.EventType, LogEventTypes.HttpRequest))
        using (LogContext.PushProperty(LogContextPropertyNames.RequestPath, context.Request.Path.Value))
        using (LogContext.PushProperty(LogContextPropertyNames.RequestMethod, context.Request.Method))
        using (LogContext.PushProperty(LogContextPropertyNames.QueryString, context.Request.QueryString.Value))
        using (LogContext.PushProperty(LogContextPropertyNames.ClientIp, clientIp))
        using (LogContext.PushProperty(LogContextPropertyNames.UserAgent, context.Request.Headers.UserAgent.ToString()))
        using (LogContext.PushProperty(LogContextPropertyNames.UserId, userId))
        using (LogContext.PushProperty(LogContextPropertyNames.UserEmail, userEmail))
        using (LogContext.PushProperty(LogContextPropertyNames.RequestBody, requestBody))
        {
            _logger.LogInformation(
                "HTTP {RequestMethod} {RequestPath} started | CorrelationId={CorrelationId} | ClientIp={ClientIp} | UserId={UserId}",
                context.Request.Method,
                context.Request.Path.Value,
                correlationId,
                clientIp,
                userId ?? "anonymous");

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                var completedUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? userId;
                var completedUserEmail = context.User.FindFirstValue(ClaimTypes.Email)
                    ?? context.User.FindFirstValue(ClaimTypes.Name)
                    ?? userEmail;

                using (LogContext.PushProperty(LogContextPropertyNames.UserId, completedUserId))
                using (LogContext.PushProperty(LogContextPropertyNames.UserEmail, completedUserEmail))
                using (LogContext.PushProperty(LogContextPropertyNames.ResponseStatusCode, context.Response.StatusCode))
                using (LogContext.PushProperty(LogContextPropertyNames.ElapsedMilliseconds, stopwatch.ElapsedMilliseconds))
                {
                    var level = context.Response.StatusCode >= 500
                        ? LogLevel.Error
                        : context.Response.StatusCode >= 400
                            ? LogLevel.Warning
                            : LogLevel.Information;

                    _logger.Log(
                        level,
                        "HTTP {RequestMethod} {RequestPath} completed | StatusCode={StatusCode} | ElapsedMs={ElapsedMs} | CorrelationId={CorrelationId} | UserId={UserId}",
                        context.Request.Method,
                        context.Request.Path.Value,
                        context.Response.StatusCode,
                        stopwatch.ElapsedMilliseconds,
                        correlationId,
                        completedUserId ?? "anonymous");
                }
            }
        }
    }

    private static bool ShouldSkip(PathString path)
    {
        var value = path.Value;
        if (string.IsNullOrWhiteSpace(value))
            return true;

        return ExcludedPaths.Any(excluded => value.StartsWith(excluded, StringComparison.OrdinalIgnoreCase));
    }

    private static async Task<string?> ReadRequestBodyAsync(HttpRequest request)
    {
        if (request.ContentLength is null or 0)
            return null;

        if (!IsLoggableContentType(request.ContentType))
            return $"[{request.ContentType ?? "unknown"} payload omitted]";

        request.EnableBuffering();

        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;

        return LogSensitiveDataSanitizer.SanitizeJson(body);
    }

    private static bool IsLoggableContentType(string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            return false;

        return contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase)
            || contentType.Contains("application/problem+json", StringComparison.OrdinalIgnoreCase)
            || contentType.Contains("text/plain", StringComparison.OrdinalIgnoreCase)
            || contentType.Contains("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase);
    }

    private static string? GetClientIp(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
            return forwardedFor.Split(',')[0].Trim();

        return context.Connection.RemoteIpAddress?.ToString();
    }
}
