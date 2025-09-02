using MediatR;
using OrderManagement.Application.Common.Models;
using OrderManagement.Application.Features.Customers.Queries.GetCustomer;

namespace OrderManagement.Application.Features.Customers.Queries.GetCustomers
{
    public record GetCustomersQuery(
     int PageNumber = 1,
     int PageSize = 10,
     string? SearchTerm = null
 ) : IRequest<Result<PaginatedList<CustomerDto>>>;

    public record CustomerDto(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string? PhoneNumber,
        DateTime CreatedAt
    );

}
