using MediatR;
using OrderManagement.Application.Common.Interfaces;
using OrderManagement.Application.Common.Models;
using OrderManagement.Application.Features.Customers.Queries.GetCustomer;
using OrderManagement.Application.Features.Customers.Specifications;
using OrderManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Features.Customers.Queries.GetCustomers
{
    public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, Result<PaginatedList<CustomerDto>>>
    {
        private readonly IRepository<Customer> _customerRepository;

        public GetCustomersQueryHandler(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Result<PaginatedList<CustomerDto>>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
        {
            var specification = new CustomerSearchSpecification(request.SearchTerm, request.PageNumber, request.PageSize);

            var customers = await _customerRepository.FindAsync(specification, cancellationToken);
            var totalCount = await _customerRepository.CountAsync(
                new CustomerSearchSpecification(request.SearchTerm),
                cancellationToken);

            var customerDtos = customers.Select(c => new CustomerDto(
                c.Id,
                c.FirstName,
                c.LastName,
                c.Email.Value,
                c.PhoneNumber,
                c.CreatedAt
            )).ToList();

            var paginatedList = PaginatedList<CustomerDto>.Create(
                customerDtos,
                request.PageNumber,
                request.PageSize,
                totalCount);

            return Result<PaginatedList<CustomerDto>>.Success(paginatedList);
        }
    }
}
