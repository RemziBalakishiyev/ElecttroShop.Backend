using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Features.Products.Commands.ChangePrice;
using ElectroShop.Application.Features.Products.Commands.ChangeStock;
using ElectroShop.Application.Features.Products.Commands.CreateProduct;
using ElectroShop.Application.Features.Products.Commands.DeleteProduct;
using ElectroShop.Application.Features.Products.Commands.UpdateProduct;
using ElectroShop.Application.Features.Products.Queries.GetProductById;
using ElectroShop.Application.Features.Products.Queries.GetProducts;
using ElectroShop.Application.Features.Products.Queries.SearchProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroShop.WebApi.Controllers;

[Authorize]
public class ProductsController : BaseApiController
{
    /// <summary>
    /// Məhsulların səhifələnmiş siyahısını əldə edir
    /// </summary>
    /// <param name="query">Səhifələmə və filtrləmə parametrləri</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Səhifələnmiş məhsul siyahısı</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetProducts(
        [FromQuery] GetProductsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(query, cancellationToken);
        return HandlePagedResult(result);
    }

    /// <summary>
    /// ID-yə görə məhsul əldə edir
    /// </summary>
    /// <param name="id">Məhsul ID-si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Məhsul detalı</returns>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery(id);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Məhsul axtarışı aparır
    /// </summary>
    /// <param name="query">Axtarış parametrləri</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Axtarış nəticələri</returns>
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchProducts(
        [FromQuery] SearchProductsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(query, cancellationToken);
        return HandlePagedResult(result);
    }

    /// <summary>
    /// Yeni məhsul yaradır
    /// </summary>
    /// <param name="command">Yaradılacaq məhsul məlumatları</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Yaradılmış məhsul</returns>
    [HttpPost]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Mövcud məhsulu yeniləyir
    /// </summary>
    /// <param name="id">Məhsul ID-si</param>
    /// <param name="command">Yenilənəcək məhsul məlumatları</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Yenilənmiş məhsul</returns>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProduct(
        [FromRoute] Guid id,
        [FromBody] UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        var updateCommand = command with { Id = id };
        var result = await Mediator.Send(updateCommand, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Məhsulu silir (Soft Delete)
    /// </summary>
    /// <param name="id">Məhsul ID-si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Uğur mesajı</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand(id);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Məhsulun qiymətini dəyişir
    /// </summary>
    /// <param name="productId">Məhsul ID-si</param>
    /// <param name="command">Yeni qiymət məlumatları</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Uğur mesajı</returns>
    [HttpPatch("{productId:guid}/price")]
    public async Task<IActionResult> ChangePrice(
        [FromRoute] Guid productId,
        [FromBody] ChangePriceCommand command,
        CancellationToken cancellationToken)
    {
        var changePriceCommand = command with { ProductId = productId };
        var result = await Mediator.Send(changePriceCommand, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Məhsulun stokunu dəyişir
    /// </summary>
    /// <param name="productId">Məhsul ID-si</param>
    /// <param name="command">Stok dəyişikliyi məlumatları</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Uğur mesajı</returns>
    [HttpPatch("{productId:guid}/stock")]
    public async Task<IActionResult> ChangeStock(
        [FromRoute] Guid productId,
        [FromBody] ChangeStockCommand command,
        CancellationToken cancellationToken)
    {
        var changeStockCommand = command with { ProductId = productId };
        var result = await Mediator.Send(changeStockCommand, cancellationToken);
        return HandleResult(result);
    }
}

