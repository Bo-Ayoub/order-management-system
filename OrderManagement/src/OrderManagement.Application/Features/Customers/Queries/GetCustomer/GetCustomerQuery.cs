using MediatR;
using OrderManagement.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Features.Customers.Queries.GetCustomer
{
    public record GetCustomerQuery(Guid CustomerId) : IRequest<Result<CustomerDto>>;

    public record CustomerDto(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string? PhoneNumber,
        DateTime CreatedAt
    );
}
