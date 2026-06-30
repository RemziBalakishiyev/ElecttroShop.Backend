using ElectroShop.Application.Features.Categories.Commands.CreateCategory;
using ElectroShop.Application.Features.Categories.Commands.DeleteCategory;
using ElectroShop.Application.Features.Categories.Commands.UpdateCategory;
using ElectroShop.Application.Features.Categories.Commands.CreateCategoryAttribute;
using ElectroShop.Application.Features.Categories.Commands.UpdateCategoryAttribute;
using ElectroShop.Application.Features.Categories.Commands.DeleteCategoryAttribute;
using ElectroShop.Application.Features.Categories.Commands.AddCategoryAttributeValue;
using ElectroShop.Application.Features.Categories.Commands.UpdateCategoryAttributeValue;
using ElectroShop.Application.Features.Categories.Commands.DeleteCategoryAttributeValue;
using ElectroShop.Application.Features.Categories.Queries.GetCategories;
using ElectroShop.Application.Features.Categories.Queries.GetCategoryById;
using ElectroShop.Application.Features.Categories.Queries.GetCategoryBySlug;
using ElectroShop.Application.Features.Categories.Queries.GetCategoryAttributes;
using ElectroShop.Application.Features.Categories.Queries.GetCategoriesLookup;
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

    /// <summary>
    /// Kateqoriya atributlarını əldə edir (dəyərlərlə birlikdə)
    /// </summary>
    [HttpGet("{categoryId:guid}/attributes")]
    public async Task<IActionResult> GetCategoryAttributes(
        [FromRoute] Guid categoryId,
        CancellationToken cancellationToken)
    {
        var query = new GetCategoryAttributesQuery(categoryId);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Kateqoriya atributu yaradır
    /// </summary>
    [HttpPost("{categoryId:guid}/attributes")]
    public async Task<IActionResult> CreateCategoryAttribute(
        [FromRoute] Guid categoryId,
        [FromBody] CreateCategoryAttributeCommand command,
        CancellationToken cancellationToken)
    {
        var createCommand = command with { CategoryId = categoryId };
        var result = await Mediator.Send(createCommand, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Kateqoriya atributunu yeniləyir
    /// </summary>
    [HttpPut("attributes/{id:guid}")]
    public async Task<IActionResult> UpdateCategoryAttribute(
        [FromRoute] Guid id,
        [FromBody] UpdateCategoryAttributeCommand command,
        CancellationToken cancellationToken)
    {
        var updateCommand = command with { Id = id };
        var result = await Mediator.Send(updateCommand, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Kateqoriya atributunu silir
    /// </summary>
    [HttpDelete("attributes/{id:guid}")]
    public async Task<IActionResult> DeleteCategoryAttribute(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryAttributeCommand(id);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Kateqoriya atributuna dəyər əlavə edir
    /// </summary>
    [HttpPost("attributes/{attributeId:guid}/values")]
    public async Task<IActionResult> AddCategoryAttributeValue(
        [FromRoute] Guid attributeId,
        [FromBody] AddCategoryAttributeValueCommand command,
        CancellationToken cancellationToken)
    {
        var addCommand = command with { CategoryAttributeId = attributeId };
        var result = await Mediator.Send(addCommand, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Kateqoriya atribut dəyərini yeniləyir
    /// </summary>
    [HttpPut("attributes/values/{id:guid}")]
    public async Task<IActionResult> UpdateCategoryAttributeValue(
        [FromRoute] Guid id,
        [FromBody] UpdateCategoryAttributeValueCommand command,
        CancellationToken cancellationToken)
    {
        var updateCommand = command with { Id = id };
        var result = await Mediator.Send(updateCommand, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Kateqoriya atribut dəyərini silir
    /// </summary>
    [HttpDelete("attributes/values/{id:guid}")]
    public async Task<IActionResult> DeleteCategoryAttributeValue(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryAttributeValueCommand(id);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Kateqoriyalar üçün lookup API - Key-Value formatında
    /// Cache management ilə - Select boxlar üçün
    /// </summary>
    /// <param name="includeAll">false olduqda yalnız root kateqoriyalar, default: bütün aktiv kateqoriyalar</param>
    /// <param name="parentId">Müəyyən parent-ın alt kateqoriyaları</param>
    [HttpGet("lookup")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategoriesLookup(
        [FromQuery] bool includeAll = true,
        [FromQuery] Guid? parentId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCategoriesLookupQuery(includeAll, parentId);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }
}

