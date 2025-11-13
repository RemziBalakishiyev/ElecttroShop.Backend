using ElectroShop.Application.Common.Results;
using FluentValidation;
using MediatR;

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

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
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
                    var method = validationResultType.GetMethod(nameof(ValidationResult<object>.WithErrors), 
                        new[] { typeof(Error[]) });
                    
                    if (method != null)
                    {
                        var validationResult = method.Invoke(null, new object[] { errors });
                        return (TResponse)validationResult!;
                    }
                }
            }

            // Fallback: throw ValidationException for non-Result responses
            throw new Exceptions.ValidationException(failures);
        }

        return await next();
    }
}

