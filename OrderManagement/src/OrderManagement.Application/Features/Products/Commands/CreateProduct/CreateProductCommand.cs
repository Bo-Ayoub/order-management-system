using MediatR;
using OrderManagement.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Features.Products.Commands.CreateProduct
{
    public record CreateProductCommand(
    string Name,
    decimal Price,
    int StockQuantity,
    string Currency = "USD",
    string? Description = null,
    string? Category = null
    ) : IRequest<Result<Guid>>;
}
