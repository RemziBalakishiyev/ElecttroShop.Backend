using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>
{
    private readonly IQueryRepository<Domain.Entities.Customer> _customerRepository;

    public GetCustomerByIdQueryHandler(IQueryRepository<Domain.Entities.Customer> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<CustomerDto>> Handle(
        GetCustomerByIdQuery request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.FirstOrDefaultAsync(
            c => c.Id == request.Id && !c.IsDeleted,
            cancellationToken);

        if (customer is null)
        {
            return DomainErrors.Customer.NotFound(request.Id);
        }

        var customerDto = customer.Adapt<CustomerDto>();

        return Result.Success(customerDto);
    }
}

