using MediatR;
using OrderManagement.Application.Common.Interfaces;
using OrderManagement.Application.Common.Models;
using OrderManagement.Application.Features.Products.Queries.GetProduct;
using OrderManagement.Application.Features.Products.Specifications;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Features.Products.Queries.GetProducts
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<PaginatedList<ProductDto>>>
    {
        private readonly IRepository<Product> _productRepository;

        public GetProductsQueryHandler(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Result<PaginatedList<ProductDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var specification = new ProductSearchSpecification(
                request.SearchTerm,
                request.Category,
                request.IsActive,
                request.MinPrice,
                request.MaxPrice,
                request.PageNumber,
                request.PageSize);

            var products = await _productRepository.FindAsync(specification, cancellationToken);
            var totalCount = await _productRepository.CountAsync(
                new ProductSearchSpecification(
                    request.SearchTerm,
                    request.Category,
                    request.IsActive,
                    request.MinPrice,
                    request.MaxPrice),
                cancellationToken);

            var productDtos = products.Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Description,
                p.Price.Amount,
                p.Price.Currency,
                p.StockQuantity,
                p.Category,
                p.IsActive,
                p.CreatedAt
            )).ToList();

            var paginatedList = PaginatedList<ProductDto>.Create(
                productDtos,
                request.PageNumber,
                request.PageSize,
                totalCount);

            return Result<PaginatedList<ProductDto>>.Success(paginatedList);
        }
    }
}
