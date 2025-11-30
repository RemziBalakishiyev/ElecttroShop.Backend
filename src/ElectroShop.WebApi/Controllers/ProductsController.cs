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
using ElectroShop.Application.Features.Products.Queries.GetBannerProduct;
using ElectroShop.Application.Features.Products.Queries.GetFeaturedProducts;
using ElectroShop.Application.Features.Products.Commands.SetProductAsBanner;
using ElectroShop.Application.Features.Products.Commands.RemoveProductFromBanner;
using ElectroShop.Application.Features.Products.Commands.SetProductAsFeatured;
using ElectroShop.Application.Features.Products.Commands.RemoveProductFromFeatured;
using ElectroShop.Application.Services;
using ElectroShop.WebApi.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroShop.WebApi.Controllers;

//[Authorize]
public class ProductsController : BaseApiController
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IImageUploadContext _imageUploadContext;

    public ProductsController(
        ILogger<ProductsController> logger,
        IImageUploadContext imageUploadContext)
    {
        _logger = logger;
        _imageUploadContext = imageUploadContext;
    }

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

    /// <summary>
    /// Məhsul üçün şəkil yükləyir
    /// </summary>
    [HttpPost("{productId:guid}/image")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadImage(
      [FromRoute] Guid productId,
      IFormFile file,
      CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is empty");

        var commandResult = await FileUploadHelper
            .CreateUploadProductImageCommandAsync(file, productId, _imageUploadContext, cancellationToken);

        if (commandResult.IsFailure)
        {
            _logger.LogWarning(
                "Image upload validation failed. ProductId: {ProductId}, Error: {Error}",
                productId, commandResult.Error.Message);

            return BadRequest(Result.Failure(commandResult.Error));
        }

        var result = await Mediator.Send(commandResult.Value, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Banner məhsulu əldə edir
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Banner məhsul</returns>
    [HttpGet("banner")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBannerProduct(CancellationToken cancellationToken)
    {
        var query = new GetBannerProductQuery();
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Featured məhsulları əldə edir (əsas səhifə üçün 5 məhsul)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Featured məhsul siyahısı</returns>
    [HttpGet("featured")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFeaturedProducts(CancellationToken cancellationToken)
    {
        var query = new GetFeaturedProductsQuery();
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Məhsulu Banner olaraq təyin edir
    /// </summary>
    /// <param name="productId">Məhsul ID-si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Uğur mesajı</returns>
    [HttpPost("{productId:guid}/banner")]
    public async Task<IActionResult> SetProductAsBanner(
        [FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        var command = new SetProductAsBannerCommand(productId);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Məhsulu Banner-dan çıxarır
    /// </summary>
    /// <param name="productId">Məhsul ID-si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Uğur mesajı</returns>
    [HttpDelete("{productId:guid}/banner")]
    public async Task<IActionResult> RemoveProductFromBanner(
        [FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveProductFromBannerCommand(productId);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Məhsulu Featured olaraq təyin edir (əsas səhifə üçün)
    /// </summary>
    /// <param name="productId">Məhsul ID-si</param>
    /// <param name="command">Display order məlumatları</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Uğur mesajı</returns>
    [HttpPost("{productId:guid}/featured")]
    public async Task<IActionResult> SetProductAsFeatured(
        [FromRoute] Guid productId,
        [FromBody] SetProductAsFeaturedCommand command,
        CancellationToken cancellationToken)
    {
        var setFeaturedCommand = command with { ProductId = productId };
        var result = await Mediator.Send(setFeaturedCommand, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Məhsulu Featured-dan çıxarır
    /// </summary>
    /// <param name="productId">Məhsul ID-si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Uğur mesajı</returns>
    [HttpDelete("{productId:guid}/featured")]
    public async Task<IActionResult> RemoveProductFromFeatured(
        [FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveProductFromFeaturedCommand(productId);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

}

