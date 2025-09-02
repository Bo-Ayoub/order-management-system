using OrderManagement.Application.Common.Specifications;
using OrderManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Features.Customers.Specifications
{
    public class CustomerByEmailSpecification : BaseSpecification<Customer>
    {
        public CustomerByEmailSpecification(string email)
            : base(c => c.Email.Value.ToLower() == email.ToLower())
        {
        }
    }

}
