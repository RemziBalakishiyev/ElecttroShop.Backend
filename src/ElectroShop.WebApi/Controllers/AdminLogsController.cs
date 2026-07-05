using ElectroShop.Application.Common.Results;
using ElectroShop.Application.Features.AppLogs.Queries.GetAppLogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroShop.WebApi.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin/logs")]
public class AdminLogsController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<Application.DTOs.AppLogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? level = null,
        [FromQuery] string? eventType = null,
        [FromQuery] string? correlationId = null,
        [FromQuery] Guid? userId = null,
        [FromQuery] string? search = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAppLogsQuery
        {
            Page = page,
            PageSize = pageSize,
            Level = level,
            EventType = eventType,
            CorrelationId = correlationId,
            UserId = userId,
            Search = search,
            DateFrom = dateFrom,
            DateTo = dateTo
        };

        var result = await Mediator.Send(query, cancellationToken);
        return HandlePagedResult(result);
    }
}
