using ElectroShop.Application.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

        return HandleFailure(result);
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
            return Ok();

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
}

