using MediatR;
using OrderManagement.Application.Common.Interfaces;
using OrderManagement.Application.Common.Models;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Features.Products.Queries.GetProduct
{
    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, Result<ProductDto>>
    {
        private readonly IRepository<Product> _productRepository;

        public GetProductQueryHandler(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Result<ProductDto>> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

            if (product == null)
                return Result<ProductDto>.Failure("Product not found");

            var productDto = new ProductDto(
                product.Id,
                product.Name,
                product.Description,
                product.Price.Amount,
                product.Price.Currency,
                product.StockQuantity,
                product.Category,
                product.IsActive,
                product.CreatedAt
            );

            return Result<ProductDto>.Success(productDto);
        }
    }
}
