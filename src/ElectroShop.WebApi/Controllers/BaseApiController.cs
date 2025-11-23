using ElectroShop.Application.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ElectroShop.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    private ISender? _mediator;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess && result.Value != null)
            return Ok(result.Value);

        if (result.IsSuccess && result.Value == null)
            return NotFound();

        // ValidationResult üçün xüsusi handling
        if (result is ValidationResult<T> validationResult)
        {
            return BadRequest(new
            {
                isSuccess = false,
                isFailure = true,
                error = new
                {
                    code = "Validation.Failed",
                    message = "Bir və ya bir neçə validasiya xətası baş verdi",
                    type = 2,
                    errors = validationResult.Errors.Select(e => new
                    {
                        code = e.Code,
                        message = e.Message,
                        property = ExtractPropertyName(e.Code)
                    }).ToArray()
                }
            });
        }

        return HandleFailure(result);
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
            return Ok();

        // ValidationResult üçün xüsusi handling
        if (result is ValidationResult validationResult)
        {
            return BadRequest(new
            {
                isSuccess = false,
                isFailure = true,
                error = new
                {
                    code = "Validation.Failed",
                    message = "Bir və ya bir neçə validasiya xətası baş verdi",
                    type = 2,
                    errors = validationResult.Errors.Select(e => new
                    {
                        code = e.Code,
                        message = e.Message,
                        property = ExtractPropertyName(e.Code)
                    }).ToArray()
                }
            });
        }

        return HandleFailure(result);
    }

    protected IActionResult HandlePagedResult<T>(PagedResult<T> result)
    {
        if (result.IsSuccess && result.Value != null && result.Value.Any())
            return Ok(result);

        if (result.IsSuccess && (result.Value == null || !result.Value.Any()))
            return Ok(result);

        return HandleFailure(result);
    }

    private IActionResult HandleFailure(Result result)
    {
        return result.Error.Type switch
        {
            ErrorType.Validation => BadRequest(result.Error),
            ErrorType.NotFound => NotFound(result.Error),
            ErrorType.Unauthorized => Unauthorized(result.Error),
            ErrorType.Forbidden => Forbid(),
            ErrorType.Conflict => Conflict(result.Error),
            _ => StatusCode(500, result.Error)
        };
    }

    private static string ExtractPropertyName(string errorCode)
    {
        // "Validation.PropertyName" formatından property name-i çıxarırıq
        if (errorCode.StartsWith("Validation."))
        {
            return errorCode.Substring("Validation.".Length);
        }
        return errorCode;
    }
}

