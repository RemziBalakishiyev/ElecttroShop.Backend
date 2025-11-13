using ElectroShop.Application.Features.Customers.Commands.RegisterCustomer;
using ElectroShop.Application.Features.Customers.Commands.UpdateCustomer;
using ElectroShop.Application.Features.Customers.Queries.GetCustomerByEmail;
using ElectroShop.Application.Features.Customers.Queries.GetCustomerById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroShop.WebApi.Controllers;

[Authorize]
public class CustomersController : BaseApiController
{
    /// <summary>
    /// ID-yə görə müştəri əldə edir
    /// </summary>
    /// <param name="id">Müştəri ID-si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Müştəri detalı</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCustomerById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetCustomerByIdQuery(id);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// E-poçt ünvanına görə müştəri əldə edir
    /// </summary>
    /// <param name="email">Müştəri e-poçt ünvanı</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Müştəri detalı</returns>
    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetCustomerByEmail(
        [FromRoute] string email,
        CancellationToken cancellationToken)
    {
        var query = new GetCustomerByEmailQuery(email);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Yeni müştəri qeydiyyatı
    /// </summary>
    /// <param name="command">Qeydiyyat məlumatları</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Qeydiyyatdan keçmiş müştəri</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterCustomer(
        [FromBody] RegisterCustomerCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Mövcud müştərini yeniləyir
    /// </summary>
    /// <param name="id">Müştəri ID-si</param>
    /// <param name="command">Yenilənəcək müştəri məlumatları</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Yenilənmiş müştəri</returns>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCustomer(
        [FromRoute] Guid id,
        [FromBody] UpdateCustomerCommand command,
        CancellationToken cancellationToken)
    {
        var updateCommand = command with { Id = id };
        var result = await Mediator.Send(updateCommand, cancellationToken);
        return HandleResult(result);
    }
}

