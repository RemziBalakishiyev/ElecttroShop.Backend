using System.Text.Json;
using ElectroShop.Application.Logging;
using ElectroShop.Domain.Entities;
using ElectroShop.Persistence.Logging;
using Serilog.Core;
using Serilog.Events;

namespace ElectroShop.WebApi.Logging;

public sealed class EfCoreLogSink : ILogEventSink
{
    private static readonly HashSet<string> MappedProperties =
    [
        LogContextPropertyNames.CorrelationId,
        LogContextPropertyNames.EventType,
        LogContextPropertyNames.UserId,
        LogContextPropertyNames.UserEmail,
        LogContextPropertyNames.RequestPath,
        LogContextPropertyNames.RequestMethod,
        LogContextPropertyNames.QueryString,
        LogContextPropertyNames.RequestBody,
        LogContextPropertyNames.ResponseStatusCode,
        LogContextPropertyNames.ElapsedMilliseconds,
        LogContextPropertyNames.ClientIp,
        LogContextPropertyNames.UserAgent,
        LogContextPropertyNames.RequestName,
        LogContextPropertyNames.RequestPayload,
        LogContextPropertyNames.ValidationErrors
    ];

    private readonly IAppLogWriter _writer;

    public EfCoreLogSink(IAppLogWriter writer)
    {
        _writer = writer;
    }

    public void Emit(LogEvent logEvent)
    {
        var entry = MapToEntry(logEvent);
        _ = _writer.EnqueueAsync(entry);
    }

    private static AppLogEntry MapToEntry(LogEvent logEvent)
    {
        var entry = new AppLogEntry
        {
            TimestampUtc = logEvent.Timestamp.UtcDateTime,
            Level = logEvent.Level.ToString(),
            Message = logEvent.RenderMessage(),
            Exception = logEvent.Exception?.ToString(),
            SourceContext = GetScalarProperty(logEvent, "SourceContext"),
            MachineName = Environment.MachineName
        };

        entry.CorrelationId = GetScalarProperty(logEvent, LogContextPropertyNames.CorrelationId);
        entry.EventType = GetScalarProperty(logEvent, LogContextPropertyNames.EventType)
            ?? InferEventType(logEvent);
        entry.UserEmail = GetScalarProperty(logEvent, LogContextPropertyNames.UserEmail);
        entry.RequestPath = GetScalarProperty(logEvent, LogContextPropertyNames.RequestPath);
        entry.RequestMethod = GetScalarProperty(logEvent, LogContextPropertyNames.RequestMethod);
        entry.QueryString = GetScalarProperty(logEvent, LogContextPropertyNames.QueryString);
        entry.RequestBody = GetScalarProperty(logEvent, LogContextPropertyNames.RequestBody)
            ?? GetScalarProperty(logEvent, LogContextPropertyNames.RequestPayload);
        entry.ClientIp = GetScalarProperty(logEvent, LogContextPropertyNames.ClientIp);
        entry.UserAgent = GetScalarProperty(logEvent, LogContextPropertyNames.UserAgent);

        if (Guid.TryParse(GetScalarProperty(logEvent, LogContextPropertyNames.UserId), out var userId))
            entry.UserId = userId;

        if (int.TryParse(GetScalarProperty(logEvent, LogContextPropertyNames.ResponseStatusCode), out var statusCode))
            entry.ResponseStatusCode = statusCode;

        if (long.TryParse(GetScalarProperty(logEvent, LogContextPropertyNames.ElapsedMilliseconds), out var elapsedMs))
            entry.ElapsedMilliseconds = elapsedMs;

        var extraProperties = new Dictionary<string, object?>();

        foreach (var property in logEvent.Properties)
        {
            if (property.Key is "SourceContext" or "RequestId" or "ActionId" or "ActionName" or "ConnectionId")
                continue;

            if (MappedProperties.Contains(property.Key))
                continue;

            extraProperties[property.Key] = property.Value.ToString();
        }

        var validationErrors = GetScalarProperty(logEvent, LogContextPropertyNames.ValidationErrors);
        if (!string.IsNullOrWhiteSpace(validationErrors))
            extraProperties[LogContextPropertyNames.ValidationErrors] = validationErrors;

        var requestName = GetScalarProperty(logEvent, LogContextPropertyNames.RequestName);
        if (!string.IsNullOrWhiteSpace(requestName))
            extraProperties[LogContextPropertyNames.RequestName] = requestName;

        if (extraProperties.Count > 0)
        {
            entry.PropertiesJson = JsonSerializer.Serialize(extraProperties);
        }

        return entry;
    }

    private static string? InferEventType(LogEvent logEvent)
    {
        var source = GetScalarProperty(logEvent, "SourceContext");

        if (source?.Contains("LoggingBehaviour", StringComparison.OrdinalIgnoreCase) == true)
            return LogEventTypes.MediatR;

        if (source?.Contains("ValidationBehaviour", StringComparison.OrdinalIgnoreCase) == true)
            return LogEventTypes.Validation;

        if (source?.Contains("RequestLoggingMiddleware", StringComparison.OrdinalIgnoreCase) == true)
            return LogEventTypes.HttpRequest;

        if (source?.Contains("ExceptionHandlingMiddleware", StringComparison.OrdinalIgnoreCase) == true)
            return LogEventTypes.Exception;

        return LogEventTypes.Application;
    }

    private static string? GetScalarProperty(LogEvent logEvent, string propertyName)
    {
        if (!logEvent.Properties.TryGetValue(propertyName, out var value))
            return null;

        return value switch
        {
            ScalarValue scalar => scalar.Value?.ToString(),
            _ => value.ToString().Trim('"')
        };
    }
}
