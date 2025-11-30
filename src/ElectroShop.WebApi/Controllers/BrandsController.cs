using ElectroShop.Application.Features.Brands.Commands.CreateBrand;
using ElectroShop.Application.Features.Brands.Commands.DeleteBrand;
using ElectroShop.Application.Features.Brands.Commands.UpdateBrand;
using ElectroShop.Application.Features.Brands.Queries.GetBrandById;
using ElectroShop.Application.Features.Brands.Queries.GetBrands;
using ElectroShop.Application.Features.Brands.Queries.GetPromotionalBrands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroShop.WebApi.Controllers;

[Authorize]
public class BrandsController : BaseApiController
{
    /// <summary>
    /// Brendlərin səhifələnmiş siyahısını əldə edir
    /// </summary>
    /// <param name="query">Səhifələmə və filtrləmə parametrləri</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Səhifələnmiş brend siyahısı</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetBrands(
        [FromQuery] GetBrandsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(query, cancellationToken);
        return HandlePagedResult(result);
    }

    /// <summary>
    /// ID-yə görə brend əldə edir
    /// </summary>
    /// <param name="id">Brend ID-si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Brend detalı</returns>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBrandById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetBrandByIdQuery(id);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Yeni brend yaradır
    /// </summary>
    /// <param name="command">Yaradılacaq brend məlumatları</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Yaradılmış brend</returns>
    [HttpPost]
    public async Task<IActionResult> CreateBrand(
        [FromBody] CreateBrandCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Mövcud brendi yeniləyir
    /// </summary>
    /// <param name="id">Brend ID-si</param>
    /// <param name="command">Yenilənəcək brend məlumatları</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Yenilənmiş brend</returns>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateBrand(
        [FromRoute] Guid id,
        [FromBody] UpdateBrandCommand command,
        CancellationToken cancellationToken)
    {
        var updateCommand = command with { Id = id };
        var result = await Mediator.Send(updateCommand, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Brendi silir (Soft Delete)
    /// </summary>
    /// <param name="id">Brend ID-si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Uğur mesajı</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBrand(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteBrandCommand(id);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Promotional brendləri və hər brend üçün featured məhsulu əldə edir
    /// Maksimum 4 brend qaytarılır (ilk 2 böyük banner, qalan 2 kiçik banner)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Promotional brendlər və featured məhsullar</returns>
    [HttpGet("promotional")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPromotionalBrands(
        CancellationToken cancellationToken)
    {
        var query = new GetPromotionalBrandsQuery();
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }
}

