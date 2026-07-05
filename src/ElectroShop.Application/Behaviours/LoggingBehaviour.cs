using ElectroShop.Application.Logging;
using ElectroShop.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ElectroShop.Application.Behaviours;

/// <summary>
/// MediatR Pipeline Behaviour for detailed request logging with timing and sanitized payload.
/// </summary>
public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUserService;

    public LoggingBehaviour(
        ILogger<LoggingBehaviour<TRequest, TResponse>> logger,
        ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var sanitizedPayload = LogSensitiveDataSanitizer.SanitizeObject(request);
        var stopwatch = Stopwatch.StartNew();

        using var scope = _logger.BeginScope(new Dictionary<string, object?>
        {
            [LogContextPropertyNames.EventType] = LogEventTypes.MediatR,
            [LogContextPropertyNames.RequestName] = requestName,
            [LogContextPropertyNames.RequestPayload] = sanitizedPayload,
            [LogContextPropertyNames.UserId] = _currentUserService.UserId?.ToString()
        });

        _logger.LogInformation(
            "MediatR handling {RequestName} | UserId={UserId} | Authenticated={IsAuthenticated} | Payload={RequestPayload}",
            requestName,
            _currentUserService.UserId?.ToString() ?? "anonymous",
            _currentUserService.IsAuthenticated,
            sanitizedPayload);

        try
        {
            var response = await next();

            stopwatch.Stop();

            _logger.LogInformation(
                "MediatR handled {RequestName} successfully in {ElapsedMilliseconds}ms | UserId={UserId}",
                requestName,
                stopwatch.ElapsedMilliseconds,
                _currentUserService.UserId?.ToString() ?? "anonymous");

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "MediatR error handling {RequestName} after {ElapsedMilliseconds}ms | UserId={UserId} | Payload={RequestPayload}",
                requestName,
                stopwatch.ElapsedMilliseconds,
                _currentUserService.UserId?.ToString() ?? "anonymous",
                sanitizedPayload);

            throw;
        }
    }
}
