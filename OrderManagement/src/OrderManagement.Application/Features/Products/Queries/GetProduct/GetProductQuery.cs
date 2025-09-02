using MediatR;
using OrderManagement.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Features.Products.Queries.GetProduct
{
    public record GetProductQuery(Guid ProductId) : IRequest<Result<ProductDto>>;

    public record ProductDto(
        Guid Id,
        string Name,
        string? Description,
        decimal Price,
        string Currency,
        int StockQuantity,
        string? Category,
        bool IsActive,
        DateTime CreatedAt
    );
}
