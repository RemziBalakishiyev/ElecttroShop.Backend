using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result<CustomerDto>>
{
    private readonly IWriteRepository<Customer> _customerRepository;
    private readonly IQueryRepository<Customer> _customerQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerCommandHandler(
        IWriteRepository<Customer> customerRepository,
        IQueryRepository<Customer> customerQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _customerQueryRepository = customerQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CustomerDto>> Handle(
        UpdateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerQueryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (customer is null)
        {
            return DomainErrors.Customer.NotFound(request.Id);
        }

        try
        {
            customer.Update(request.FullName, request.Email, request.Phone);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<CustomerDto>(Error.Validation("Customer.InvalidData", ex.Message));
        }

        _customerRepository.Update(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var customerDto = new CustomerDto
        {
            Id = customer.Id,
            FullName = customer.FullName,
            Email = customer.Email,
            Phone = customer.Phone,
            CreatedAt = customer.CreatedAtUtc,
            UpdatedAt = customer.UpdatedAtUtc
        };

        return Result.Success(customerDto);
    }
}

