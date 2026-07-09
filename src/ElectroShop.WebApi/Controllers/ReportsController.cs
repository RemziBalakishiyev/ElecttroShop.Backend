using ElectroShop.Application.DTOs;
using ElectroShop.Application.Features.Reports.Queries.GetMonthlySalesReport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroShop.WebApi.Controllers;

/// <summary>
/// Hesabatlar modulu üçün Controller (Admin panel)
/// </summary>
[Authorize]
[ApiController]
[Route("api/reports")]
public class ReportsController : BaseApiController
{
    /// <summary>
    /// Seçilmiş ay üzrə satış hesabatını JSON formatında qaytarır (dashboard üçün)
    /// </summary>
    [HttpGet("sales/monthly")]
    [ProducesResponseType(typeof(MonthlySalesReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMonthlySalesReport(
        [FromQuery] int year,
        [FromQuery] int month,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetMonthlySalesReportQuery(year, month), cancellationToken);
        return HandleResult(result);
    }
}
