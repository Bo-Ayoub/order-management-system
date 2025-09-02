using MediatR;
using OrderManagement.Application.Common.Interfaces;
using OrderManagement.Application.Common.Models;
using OrderManagement.Application.Features.Customers.Specifications;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Application.Features.Customers.Commands.CreateCustomer
{
    public  class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<Guid>>
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCustomerCommandHandler(
            IRepository<Customer> customerRepository,
            IUnitOfWork unitOfWork)
        {
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if customer with email already exists
                var emailExists = await _customerRepository.ExistsAsync(
                    new CustomerByEmailSpecification(request.Email),
                    cancellationToken);

                if (emailExists)
                {
                    return Result<Guid>.Failure("A customer with this email already exists");
                }

                var email = new Email(request.Email);
                var customer = new Customer(
                    request.FirstName,
                    request.LastName,
                    email,
                    request.PhoneNumber);

                await _customerRepository.AddAsync(customer, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(customer.Id);
            }
            catch (ArgumentException ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"Failed to create customer: {ex.Message}");
            }
        }
    }
}
