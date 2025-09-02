using MediatR;
using OrderManagement.Application.Common.Models;

public record GetProductsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? Category = null,
    bool? IsActive = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null
) : IRequest<Result<PaginatedList<ProductDto>>>;

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