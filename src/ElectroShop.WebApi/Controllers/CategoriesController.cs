using ElectroShop.Application.Features.Categories.Commands.CreateCategory;
using ElectroShop.Application.Features.Categories.Commands.DeleteCategory;
using ElectroShop.Application.Features.Categories.Commands.UpdateCategory;
using ElectroShop.Application.Features.Categories.Queries.GetCategories;
using ElectroShop.Application.Features.Categories.Queries.GetCategoryById;
using ElectroShop.Application.Features.Categories.Queries.GetCategoryBySlug;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroShop.WebApi.Controllers;

[Authorize]
public class CategoriesController : BaseApiController
{
    /// <summary>
    /// Kateqoriyaların səhifələnmiş siyahısını əldə edir
    /// </summary>
    /// <param name="query">Səhifələmə və filtrləmə parametrləri</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Səhifələnmiş kateqoriya siyahısı</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategories(
        [FromQuery] GetCategoriesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(query, cancellationToken);
        return HandlePagedResult(result);
    }

    /// <summary>
    /// ID-yə görə kateqoriya əldə edir
    /// </summary>
    /// <param name="id">Kateqoriya ID-si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Kateqoriya detalı</returns>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategoryById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetCategoryByIdQuery(id);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Slug-a görə kateqoriya əldə edir
    /// </summary>
    /// <param name="slug">Kateqoriya slug-u</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Kateqoriya detalı</returns>
    [HttpGet("slug/{slug}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategoryBySlug(
        [FromRoute] string slug,
        CancellationToken cancellationToken)
    {
        var query = new GetCategoryBySlugQuery(slug);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Yeni kateqoriya yaradır
    /// </summary>
    /// <param name="command">Yaradılacaq kateqoriya məlumatları</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Yaradılmış kateqoriya</returns>
    [HttpPost]
    public async Task<IActionResult> CreateCategory(
        [FromBody] CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Mövcud kateqoriyanı yeniləyir
    /// </summary>
    /// <param name="id">Kateqoriya ID-si</param>
    /// <param name="command">Yenilənəcək kateqoriya məlumatları</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Yenilənmiş kateqoriya</returns>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCategory(
        [FromRoute] Guid id,
        [FromBody] UpdateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var updateCommand = command with { Id = id };
        var result = await Mediator.Send(updateCommand, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Kateqoriyanı silir (Soft Delete)
    /// </summary>
    /// <param name="id">Kateqoriya ID-si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Uğur mesajı</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCategory(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryCommand(id);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}

