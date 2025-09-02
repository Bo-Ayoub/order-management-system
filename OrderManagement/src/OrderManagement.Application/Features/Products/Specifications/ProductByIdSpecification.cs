using OrderManagement.Application.Common.Specifications;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Features.Products.Specifications;

public class ProductByIdSpecification : BaseSpecification<Product>
{
    public ProductByIdSpecification(Guid productId)
        : base(p => p.Id == productId)
    {
    }
}