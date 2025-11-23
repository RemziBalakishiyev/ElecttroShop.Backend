using ElectroShop.Application.DTOs;
using ElectroShop.Application.Features.Dashboard.Queries.GetChartData;
using ElectroShop.Application.Features.Dashboard.Queries.GetDashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroShop.WebApi.Controllers;

/// <summary>
/// Dashboard üçün Controller
/// </summary>
//[Authorize]
[ApiController]
[Route("api/dashboard")]
public class DashboardController : BaseApiController
{
    /// <summary>
    /// Dashboard statistikalarını və məlumatlarını əldə edir
    /// </summary>
    /// <returns>Dashboard məlumatları (statistikalar, son məhsullar, son sifarişlər)</returns>
    [HttpGet]
    [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var query = new GetDashboardQuery();
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Chart məlumatlarını əldə edir
    /// </summary>
    /// <param name="period">Zaman intervalı: "daily", "weekly", "monthly" (default: "monthly")</param>
    /// <param name="periodCount">Neçə period geriyə getmək (default: 12)</param>
    /// <returns>Chart məlumatları (gəlir, sifariş sayı, kateqoriya satışları, status paylanması, top məhsullar)</returns>
    [HttpGet("chart")]
    [ProducesResponseType(typeof(ChartDataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetChartData(
        [FromQuery] string period = "monthly",
        [FromQuery] int periodCount = 12,
        CancellationToken cancellationToken = default)
    {
        var query = new GetChartDataQuery
        {
            Period = period,
            PeriodCount = periodCount
        };
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }
}

