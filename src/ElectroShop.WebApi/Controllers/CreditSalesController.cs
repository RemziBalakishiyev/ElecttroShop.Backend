using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Features.CreditSales.Commands.CancelCreditSale;
using ElectroShop.Application.Features.CreditSales.Commands.CreateCreditSale;
using ElectroShop.Application.Features.CreditSales.Commands.MarkCreditSaleAsSold;
using ElectroShop.Application.Features.CreditSales.Commands.UpdateCreditSale;
using ElectroShop.Application.Features.CreditSales.Queries.GetCreditSaleById;
using ElectroShop.Application.Features.CreditSales.Queries.GetCreditSales;
using ElectroShop.Application.Features.CreditSales.Queries.GetCreditSalesSummary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroShop.WebApi.Controllers;

/// <summary>
/// Nisyə (kredit satış) idarəetməsi üçün Controller (Admin panel)
/// </summary>
[Authorize]
[ApiController]
[Route("api/credit-sales")]
public class CreditSalesController : BaseApiController
{
    /// <summary>
    /// Nisyələrin səhifələnmiş siyahısını əldə edir
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CreditSaleListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCreditSales(
        [FromQuery] GetCreditSalesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(query, cancellationToken);
        return HandlePagedResult(result);
    }

    /// <summary>
    /// Nisyələr üzrə xülasə statistikalarını əldə edir
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(CreditSaleSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCreditSalesSummary(
        [FromQuery] GetCreditSalesSummaryQuery query,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// ID-yə görə nisyə detalı əldə edir
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CreditSaleDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCreditSaleById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetCreditSaleByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Yeni nisyə yaradır
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreditSaleDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateCreditSale(
        [FromBody] CreateCreditSaleCommand? command,
        CancellationToken cancellationToken)
    {
        if (command is null)
        {
            return BadRequest(new
            {
                code = "CreditSale.InvalidRequest",
                message = "Request body oxuna bilmədi. productSourceType 'Manual' və ya 'SystemProduct' (və ya 1/2), dueDate və digər məcburi sahələri yoxlayın."
            });
        }

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Nisyə qeydini yeniləyir
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CreditSaleDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateCreditSale(
        [FromRoute] Guid id,
        [FromBody] UpdateCreditSaleCommand? command,
        CancellationToken cancellationToken)
    {
        if (command is null)
        {
            return BadRequest(new
            {
                code = "CreditSale.InvalidRequest",
                message = "Request body oxuna bilmədi. dueDate və digər məcburi sahələri yoxlayın."
            });
        }

        var updateCommand = command with { Id = id };
        var result = await Mediator.Send(updateCommand, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Nisyəni satılmış kimi qeyd edir və satış moduluna çevirir
    /// </summary>
    [HttpPost("{id:guid}/mark-as-sold")]
    [ProducesResponseType(typeof(CreditSaleDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkCreditSaleAsSold(
        [FromRoute] Guid id,
        [FromBody] MarkCreditSaleAsSoldCommand? command,
        CancellationToken cancellationToken)
    {
        var markCommand = (command ?? new MarkCreditSaleAsSoldCommand()) with { Id = id };
        var result = await Mediator.Send(markCommand, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Nisyəni ləğv edir
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CancelCreditSale(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new CancelCreditSaleCommand(id), cancellationToken);
        return HandleResult(result);
    }
}
