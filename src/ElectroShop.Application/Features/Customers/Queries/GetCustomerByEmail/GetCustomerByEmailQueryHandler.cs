using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.Customers.Queries.GetCustomerByEmail;

public class GetCustomerByEmailQueryHandler : IRequestHandler<GetCustomerByEmailQuery, Result<CustomerDto>>
{
    private readonly ICustomerQueryRepository _customerRepository;

    public GetCustomerByEmailQueryHandler(ICustomerQueryRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<CustomerDto>> Handle(
        GetCustomerByEmailQuery request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetCustomerByEmailAsync(request.Email, cancellationToken);

        if (customer is null)
        {
            return Result.Failure<CustomerDto>(
                Error.NotFound("Customer.NotFoundByEmail", $"E-poçt '{request.Email}' ilə müştəri tapılmadı"));
        }

        var customerDto = customer.Adapt<CustomerDto>();

        return Result.Success(customerDto);
    }
}

