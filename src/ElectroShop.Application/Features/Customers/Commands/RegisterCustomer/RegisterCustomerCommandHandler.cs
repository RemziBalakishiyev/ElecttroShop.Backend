using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Customers.Commands.RegisterCustomer;

public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, Result<CustomerDto>>
{
    private readonly IWriteRepository<Customer> _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCustomerCommandHandler(
        IWriteRepository<Customer> customerRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CustomerDto>> Handle(
        RegisterCustomerCommand request,
        CancellationToken cancellationToken)
    {
        Customer customer;
        try
        {
            customer = Customer.Create(request.FullName, request.Email, request.Phone);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<CustomerDto>(Error.Validation("Customer.InvalidData", ex.Message));
        }

        await _customerRepository.AddAsync(customer, cancellationToken);
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

