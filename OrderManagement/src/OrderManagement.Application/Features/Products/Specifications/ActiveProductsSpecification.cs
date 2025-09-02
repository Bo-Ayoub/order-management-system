using OrderManagement.Application.Common.Specifications;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Features.Products.Specifications;

public class ActiveProductsSpecification : BaseSpecification<Product>
{
    public ActiveProductsSpecification()
        : base(p => p.IsActive)
    {
        ApplyOrderBy(p => p.Name);
    }
}