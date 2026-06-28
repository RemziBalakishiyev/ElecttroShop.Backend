using ElectroShop.Application.DTOs;
using ElectroShop.Application.Features.ProductRatings.Commands.CreateProductRating;
using ElectroShop.Application.Features.ProductRatings.Commands.DeleteProductRating;
using ElectroShop.Application.Features.ProductRatings.Commands.UpdateProductRating;
using ElectroShop.Application.Features.ProductRatings.Queries.GetMyProductRating;
using ElectroShop.Application.Features.ProductRatings.Queries.GetProductRatingSummary;
using ElectroShop.Application.Features.ProductRatings.Queries.GetProductRatings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroShop.WebApi.Controllers;

[Route("api/Products/{productId:guid}/ratings")]
public class ProductRatingsController : BaseApiController
{
    /// <summary>
    /// Məhsul reytinqlərinin səhifələnmiş siyahısı
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductRatings(
        [FromRoute] Guid productId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductRatingsQuery
        {
            ProductId = productId,
            Page = page,
            PageSize = pageSize
        };

        var result = await Mediator.Send(query, cancellationToken);
        return HandlePagedResult(result);
    }

    /// <summary>
    /// Məhsul reytinq xülasəsi (ortalama, say, cari istifadəçi reytinqi)
    /// </summary>
    [HttpGet("summary")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductRatingSummary(
        [FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        var query = new GetProductRatingSummaryQuery(productId);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Cari istifadəçinin məhsul üçün reytinqini əldə edir
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMyProductRating(
        [FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        var query = new GetMyProductRatingQuery(productId);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Məhsula reytinq verir
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateProductRating(
        [FromRoute] Guid productId,
        [FromBody] CreateProductRatingRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateProductRatingCommand(productId, request.RatingValue, request.Comment);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Cari istifadəçinin məhsul reytinqini yeniləyir
    /// </summary>
    [HttpPut("me")]
    [Authorize]
    public async Task<IActionResult> UpdateMyProductRating(
        [FromRoute] Guid productId,
        [FromBody] UpdateProductRatingRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductRatingCommand(productId, request.RatingValue, request.Comment);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Cari istifadəçinin məhsul reytinqini silir (soft delete)
    /// </summary>
    [HttpDelete("me")]
    [Authorize]
    public async Task<IActionResult> DeleteMyProductRating(
        [FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProductRatingCommand(productId);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
