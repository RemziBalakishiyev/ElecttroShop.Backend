using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Customers.Commands.UpdateCustomer;

public record UpdateCustomerCommand(
    Guid Id,
    string FullName,
    string Email,
    string? Phone = null) : IRequest<Result<CustomerDto>>;

