using OrderManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace OrderManagement.Application.Common.Interfaces
{
    public  interface IApplicationDbContext
    {
        DbSet<Customer> Customers { get; }
        DbSet<Product> Products { get; }
        DbSet<Order> Orders { get; }
        DbSet<OrderItem> OrderItems { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
