using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Customers.Commands.RegisterCustomer;

public record RegisterCustomerCommand(
    string FullName,
    string Email,
    string? Phone = null) : IRequest<Result<CustomerDto>>;

