using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Features.Sales.Commands.CreateSale;
using ElectroShop.Application.Features.Sales.Commands.DeleteSale;
using ElectroShop.Application.Features.Sales.Commands.UpdateSale;
using ElectroShop.Application.Features.Sales.Queries.GetSaleById;
using ElectroShop.Application.Features.Sales.Queries.GetSales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroShop.WebApi.Controllers;

/// <summary>
/// Satış idarəetməsi üçün Controller (Admin panel)
/// </summary>
[Authorize]
[ApiController]
[Route("api/sales")]
public class SalesController : BaseApiController
{
    /// <summary>
    /// Satışların səhifələnmiş siyahısını əldə edir
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SaleListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSales(
        [FromQuery] GetSalesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(query, cancellationToken);
        return HandlePagedResult(result);
    }

    /// <summary>
    /// ID-yə görə satış detalı əldə edir
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SaleDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSaleById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetSaleByIdQuery(id);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Yeni satış yaradır (mövcud məhsul və ya manual giriş)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SaleDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateSale(
        [FromBody] CreateSaleCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Satış qeydini yeniləyir
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SaleDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateSale(
        [FromRoute] Guid id,
        [FromBody] UpdateSaleCommand command,
        CancellationToken cancellationToken)
    {
        var updateCommand = command with { Id = id };
        var result = await Mediator.Send(updateCommand, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Satış qeydini silir (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteSale(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteSaleCommand(id);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
