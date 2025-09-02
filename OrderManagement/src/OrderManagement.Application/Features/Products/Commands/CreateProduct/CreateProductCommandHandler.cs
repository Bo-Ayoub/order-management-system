using MediatR;
using OrderManagement.Application.Common.Interfaces;
using OrderManagement.Application.Common.Models;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductCommandHandler(
            IRepository<Product> productRepository,
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var price = new Money(request.Price, request.Currency);
                var product = new Product(
                    request.Name,
                    price,
                    request.StockQuantity,
                    request.Description,
                    request.Category);

                await _productRepository.AddAsync(product, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(product.Id);
            }
            catch (ArgumentException ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"Failed to create product: {ex.Message}");
            }
        }
    }
}

