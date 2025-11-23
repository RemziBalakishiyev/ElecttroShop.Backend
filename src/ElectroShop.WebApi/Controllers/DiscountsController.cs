using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Features.Discounts.Commands.CreateDiscount;
using ElectroShop.Application.Features.Discounts.Commands.DeleteDiscount;
using ElectroShop.Application.Features.Discounts.Commands.UpdateDiscount;
using ElectroShop.Application.Features.Discounts.Queries.GetDiscountById;
using ElectroShop.Application.Features.Discounts.Queries.GetDiscounts;
using ElectroShop.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroShop.WebApi.Controllers;

/// <summary>
/// Endirim idarəetməsi üçün Controller
/// </summary>
[Authorize]
[ApiController]
[Route("api/discounts")]
public class DiscountsController : BaseApiController
{
    /// <summary>
    /// Endirimlərin səhifələnmiş siyahısını əldə edir
    /// </summary>
    /// <param name="query">Səhifələmə və filtrləmə parametrləri</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Səhifələnmiş endirim siyahısı</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<DiscountListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDiscounts(
        [FromQuery] GetDiscountsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(query, cancellationToken);
        return HandlePagedResult(result);
    }

    /// <summary>
    /// ID-yə görə endirim əldə edir
    /// </summary>
    /// <param name="id">Endirim ID-si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Endirim detalı</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DiscountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDiscountById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetDiscountByIdQuery(id);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Yeni endirim yaradır
    /// </summary>
    /// <param name="command">Yaradılacaq endirim məlumatları</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Yaradılmış endirim</returns>
    [HttpPost]
    [ProducesResponseType(typeof(DiscountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateDiscount(
        [FromBody] CreateDiscountCommand? command,
        CancellationToken cancellationToken)
    {
        if (command == null)
        {
            return BadRequest(new
            {
                isSuccess = false,
                isFailure = true,
                error = new
                {
                    code = "Validation.ArgumentError",
                    message = "Request body boş ola bilməz",
                    type = 2
                }
            });
        }

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Mövcud endirimi yeniləyir
    /// </summary>
    /// <param name="id">Endirim ID-si</param>
    /// <param name="command">Yenilənəcək endirim məlumatları</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Yenilənmiş endirim</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(DiscountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateDiscount(
        [FromRoute] Guid id,
        [FromBody] UpdateDiscountCommand? command,
        CancellationToken cancellationToken)
    {
        if (command == null)
        {
            return BadRequest(new
            {
                isSuccess = false,
                isFailure = true,
                error = new
                {
                    code = "Validation.ArgumentError",
                    message = "Request body boş ola bilməz",
                    type = 2
                }
            });
        }

        var updateCommand = command with { Id = id };
        var result = await Mediator.Send(updateCommand, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Endirimi silir (Soft Delete - deaktiv edir)
    /// </summary>
    /// <param name="id">Endirim ID-si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Uğur mesajı</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteDiscount(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteDiscountCommand(id);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}

