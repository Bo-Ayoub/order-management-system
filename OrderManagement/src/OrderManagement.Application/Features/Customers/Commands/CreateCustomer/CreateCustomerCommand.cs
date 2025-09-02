using MediatR;
using OrderManagement.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Features.Customers.Commands.CreateCustomer
{
    public record CreateCustomerCommand(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber = null
    ) : IRequest<Result<Guid>>;
}
