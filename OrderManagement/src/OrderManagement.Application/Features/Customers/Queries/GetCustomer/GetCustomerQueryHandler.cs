using MediatR;
using OrderManagement.Application.Common.Interfaces;
using OrderManagement.Application.Common.Models;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Features.Customers.Queries.GetCustomer
{
    public class GetCustomerQueryHandler : IRequestHandler<GetCustomerQuery, Result<CustomerDto>>
    {
        private readonly IRepository<Customer> _customerRepository;

        public GetCustomerQueryHandler(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Result<CustomerDto>> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);

            if (customer == null)
                return Result<CustomerDto>.Failure("Customer not found");

            var customerDto = new CustomerDto(
                customer.Id,
                customer.FirstName,
                customer.LastName,
                customer.Email.Value,
                customer.PhoneNumber,
                customer.CreatedAt
            );

            return Result<CustomerDto>.Success(customerDto);
        }
    }
}
