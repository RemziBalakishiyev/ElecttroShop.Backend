using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Customers.Queries.GetCustomerByEmail;

public record GetCustomerByEmailQuery(string Email) : IRequest<Result<CustomerDto>>;

