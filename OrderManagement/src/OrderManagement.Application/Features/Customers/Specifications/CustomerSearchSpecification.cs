using OrderManagement.Application.Common.Specifications;
using OrderManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Features.Customers.Specifications
{
    public class CustomerSearchSpecification : BaseSpecification<Customer>
    {
        public CustomerSearchSpecification(string? searchTerm = null, int? pageNumber = null, int? pageSize = null)
            : base(BuildCriteria(searchTerm))
        {
            // Default ordering by creation date
            ApplyOrderByDescending(c => c.CreatedAt);

            // Apply pagination if provided
            if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
            {
                ApplyPaging((pageNumber.Value - 1) * pageSize.Value, pageSize.Value);
            }
        }

        private static Expression<Func<Customer, bool>>? BuildCriteria(string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return null;

            var lowerSearchTerm = searchTerm.ToLower();
            return c => c.FirstName.ToLower().Contains(lowerSearchTerm) ||
                       c.LastName.ToLower().Contains(lowerSearchTerm) ||
                       c.Email.Value.ToLower().Contains(lowerSearchTerm);
        }
    }
}
