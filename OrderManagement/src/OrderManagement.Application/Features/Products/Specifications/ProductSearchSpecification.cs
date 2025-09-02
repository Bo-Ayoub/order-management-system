using OrderManagement.Application.Common.Specifications;
using OrderManagement.Domain.Entities;
using System.Linq.Expressions;

namespace OrderManagement.Application.Features.Products.Specifications;

public class ProductSearchSpecification : BaseSpecification<Product>
{
    public ProductSearchSpecification(
        string? searchTerm = null,
        string? category = null,
        bool? isActive = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int? pageNumber = null,
        int? pageSize = null)
        : base(BuildCriteria(searchTerm, category, isActive, minPrice, maxPrice))
    {
        // Default ordering by name
        ApplyOrderBy(p => p.Name);

        // Apply pagination if provided
        if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
        {
            ApplyPaging((pageNumber.Value - 1) * pageSize.Value, pageSize.Value);
        }
    }

    private static Expression<Func<Product, bool>>? BuildCriteria(
        string? searchTerm,
        string? category,
        bool? isActive,
        decimal? minPrice,
        decimal? maxPrice)
    {
        Expression<Func<Product, bool>>? criteria = null;

        // Search term filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            Expression<Func<Product, bool>> searchCriteria = p =>
                p.Name.ToLower().Contains(lowerSearchTerm) ||
                (p.Description != null && p.Description.ToLower().Contains(lowerSearchTerm));

            criteria = CombineWithAnd(criteria, searchCriteria);
        }

        // Category filter
        if (!string.IsNullOrWhiteSpace(category))
        {
            Expression<Func<Product, bool>> categoryCriteria = p =>
                p.Category != null && p.Category.ToLower() == category.ToLower();

            criteria = CombineWithAnd(criteria, categoryCriteria);
        }

        // Active status filter
        if (isActive.HasValue)
        {
            Expression<Func<Product, bool>> activeCriteria = p => p.IsActive == isActive.Value;
            criteria = CombineWithAnd(criteria, activeCriteria);
        }

        // Price range filters
        if (minPrice.HasValue)
        {
            Expression<Func<Product, bool>> minPriceCriteria = p => p.Price.Amount >= minPrice.Value;
            criteria = CombineWithAnd(criteria, minPriceCriteria);
        }

        if (maxPrice.HasValue)
        {
            Expression<Func<Product, bool>> maxPriceCriteria = p => p.Price.Amount <= maxPrice.Value;
            criteria = CombineWithAnd(criteria, maxPriceCriteria);
        }

        return criteria;
    }

    private static Expression<Func<Product, bool>>? CombineWithAnd(
        Expression<Func<Product, bool>>? left,
        Expression<Func<Product, bool>> right)
    {
        if (left == null)
            return right;

        var parameter = Expression.Parameter(typeof(Product), "p");
        var leftInvoke = Expression.Invoke(left, parameter);
        var rightInvoke = Expression.Invoke(right, parameter);
        var andExpression = Expression.AndAlso(leftInvoke, rightInvoke);

        return Expression.Lambda<Func<Product, bool>>(andExpression, parameter);
    }
}