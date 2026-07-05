using ElectroShop.Application.Common.Results;
using ElectroShop.Application.Logging;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace ElectroShop.Application.Behaviours;

/// <summary>
/// MediatR Pipeline Behaviour for validating requests using FluentValidation
/// Works seamlessly with Result pattern
/// </summary>
/// <typeparam name="TRequest">The request type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehaviour<TRequest, TResponse>> _logger;

    public ValidationBehaviour(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationBehaviour<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Any())
        {
            var requestName = typeof(TRequest).Name;
            var validationSummary = string.Join("; ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}"));
            var sanitizedPayload = LogSensitiveDataSanitizer.SanitizeObject(request);

            using (_logger.BeginScope(new Dictionary<string, object?>
            {
                [LogContextPropertyNames.EventType] = LogEventTypes.Validation,
                [LogContextPropertyNames.RequestName] = requestName,
                [LogContextPropertyNames.ValidationErrors] = validationSummary,
                [LogContextPropertyNames.RequestPayload] = sanitizedPayload
            }))
            {
                _logger.LogWarning(
                    "Validation failed for {RequestName} | Errors={ValidationErrors} | Payload={RequestPayload}",
                    requestName,
                    validationSummary,
                    sanitizedPayload);
            }

            // Convert FluentValidation errors to Result pattern errors
            var errors = failures
                .Select(f => Error.Validation(
                    $"Validation.{f.PropertyName}",
                    f.ErrorMessage))
                .ToArray();

            // If TResponse is a Result type, return a validation result
            if (typeof(TResponse).IsGenericType)
            {
                var genericType = typeof(TResponse).GetGenericTypeDefinition();
                
                if (genericType == typeof(Result<>))
                {
                    var resultType = typeof(TResponse).GetGenericArguments()[0];
                    var validationResultType = typeof(ValidationResult<>).MakeGenericType(resultType);
                    
                    // WithErrors metodunu tapırıq (Error[] və ya IEnumerable<Error> overload-ları)
                    var method = validationResultType.GetMethod(nameof(ValidationResult<object>.WithErrors), 
                        new[] { typeof(Error[]) }) 
                        ?? validationResultType.GetMethod(nameof(ValidationResult<object>.WithErrors), 
                        new[] { typeof(IEnumerable<Error>) });
                    
                    if (method != null)
                    {
                        var validationResult = method.Invoke(null, new object[] { errors });
                        return (TResponse)validationResult!;
                    }
                }
            }
            
            // Non-generic Result üçün
            if (typeof(TResponse) == typeof(Result))
            {
                return (TResponse)(object)ValidationResult.WithErrors(errors);
            }

            // Fallback: throw ValidationException for non-Result responses
            throw new Exceptions.ValidationException(failures);
        }

        return await next();
    }
}

