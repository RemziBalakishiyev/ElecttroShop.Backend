using ElectroShop.Application.Features.Orders.Commands.AddOrderItem;
using ElectroShop.Application.Features.Orders.Commands.CreateOrder;
using ElectroShop.Application.Features.Orders.Commands.MarkOrderPaid;
using ElectroShop.Application.Features.Orders.Commands.RemoveOrderItem;
using ElectroShop.Application.Features.Orders.Queries.GetOrderById;
using ElectroShop.Application.Features.Orders.Queries.GetOrdersByCustomer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroShop.WebApi.Controllers;

[Authorize]
public class OrdersController : BaseApiController
{
    /// <summary>
    /// ID-yə görə sifariş əldə edir
    /// </summary>
    /// <param name="id">Sifariş ID-si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sifariş detalı</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrderById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdQuery(id);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Müştəriyə görə səhifələnmiş sifariş siyahısını əldə edir
    /// </summary>
    /// <param name="customerId">Müştəri ID-si</param>
    /// <param name="query">Səhifələmə parametrləri</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Səhifələnmiş sifariş siyahısı</returns>
    [HttpGet("customer/{customerId:guid}")]
    public async Task<IActionResult> GetOrdersByCustomer(
        [FromRoute] Guid customerId,
        [FromQuery] GetOrdersByCustomerQuery query,
        CancellationToken cancellationToken)
    {
        var ordersQuery = query with { CustomerId = customerId };
        var result = await Mediator.Send(ordersQuery, cancellationToken);
        return HandlePagedResult(result);
    }

    /// <summary>
    /// Yeni sifariş yaradır
    /// </summary>
    /// <param name="command">Yaradılacaq sifariş məlumatları</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Yaradılmış sifariş</returns>
    [HttpPost]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Sifarişə məhsul əlavə edir
    /// </summary>
    /// <param name="orderId">Sifariş ID-si</param>
    /// <param name="command">Əlavə ediləcək məhsul məlumatları</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Yenilənmiş sifariş</returns>
    [HttpPost("{orderId:guid}/items")]
    public async Task<IActionResult> AddOrderItem(
        [FromRoute] Guid orderId,
        [FromBody] AddOrderItemCommand command,
        CancellationToken cancellationToken)
    {
        var addItemCommand = command with { OrderId = orderId };
        var result = await Mediator.Send(addItemCommand, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Sifarişdən məhsul silir
    /// </summary>
    /// <param name="orderId">Sifariş ID-si</param>
    /// <param name="productId">Məhsul ID-si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Yenilənmiş sifariş</returns>
    [HttpDelete("{orderId:guid}/items/{productId:guid}")]
    public async Task<IActionResult> RemoveOrderItem(
        [FromRoute] Guid orderId,
        [FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveOrderItemCommand(orderId, productId);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Sifarişi ödənilmiş olaraq qeyd edir
    /// </summary>
    /// <param name="id">Sifariş ID-si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Yenilənmiş sifariş</returns>
    [HttpPatch("{id:guid}/mark-paid")]
    public async Task<IActionResult> MarkOrderPaid(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new MarkOrderPaidCommand(id);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}

