using OrderManagement.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace OrderManagement.Infrastructure.Data.Extensions
{
    public static class SpecificationEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification, bool evaluateCriteriaOnly = false)
        {
            var query = inputQuery;

            // Apply criteria (WHERE clause)
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            // Return early if only evaluating criteria (for Count operations)
            if (evaluateCriteriaOnly)
            {
                return query;
            }

            // Apply Includes
            query = specification.Includes
                .Aggregate(query, (current, include) => current.Include(include));

            // Apply string-based includes (for nested includes)
            query = specification.IncludeStrings
                .Aggregate(query, (current, include) => current.Include(include));

            // Apply ordering
            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            // Apply grouping
            if (specification.GroupBy != null)
            {
                query = query.GroupBy(specification.GroupBy).SelectMany(x => x);
            }

            // Apply paging
            if (specification.IsPagingEnabled)
            {
                query = query.Skip(specification.Skip).Take(specification.Take);
            }

            return query;
        }
    }
}
