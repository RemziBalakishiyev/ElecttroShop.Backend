namespace ElectroShop.Application.Logging;

public static class LogContextPropertyNames
{
    public const string CorrelationId = "CorrelationId";
    public const string EventType = "EventType";
    public const string UserId = "UserId";
    public const string UserEmail = "UserEmail";
    public const string RequestPath = "RequestPath";
    public const string RequestMethod = "RequestMethod";
    public const string QueryString = "QueryString";
    public const string RequestBody = "RequestBody";
    public const string ResponseStatusCode = "ResponseStatusCode";
    public const string ElapsedMilliseconds = "ElapsedMilliseconds";
    public const string ClientIp = "ClientIp";
    public const string UserAgent = "UserAgent";
    public const string RequestName = "RequestName";
    public const string RequestPayload = "RequestPayload";
    public const string ValidationErrors = "ValidationErrors";
}

public static class LogEventTypes
{
    public const string HttpRequest = "HttpRequest";
    public const string MediatR = "MediatR";
    public const string Validation = "Validation";
    public const string Exception = "Exception";
    public const string Application = "Application";
    public const string ImageStorage = "ImageStorage";
    public const string DomainEvent = "DomainEvent";
}
