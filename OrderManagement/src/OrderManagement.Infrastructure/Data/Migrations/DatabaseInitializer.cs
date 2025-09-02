using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Infrastructure.Data.Migrations
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

            try
            {
                // Ensure database is created
                await context.Database.EnsureCreatedAsync();

                // Apply any pending migrations
                if (context.Database.GetPendingMigrations().Any())
                {
                    await context.Database.MigrateAsync();
                }

                // Seed initial data if needed
                await SeedDataAsync(context);

                logger.LogInformation("Database initialization completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database");
                throw;
            }
        }

        private static async Task SeedDataAsync(ApplicationDbContext context)
        {
            // Seed customers if none exist
            if (!await context.Customers.AnyAsync())
            {
                var customers = new[]
                {
                new Customer("John", "Doe", new Email("john.doe@example.com"), "+1-555-0101"),
                new Customer("Jane", "Smith", new Email("jane.smith@example.com"), "+1-555-0102"),
                new Customer("Bob", "Johnson", new Email("bob.johnson@example.com"), "+1-555-0103"),
            };

                context.Customers.AddRange(customers);
            }

            // Seed products if none exist
            if (!await context.Products.AnyAsync())
            {
                var products = new[]
                {
                new Product("Laptop Pro 16", new Money(2499.99m, "USD"), 50, "High-performance laptop for professionals", "Electronics"),
                new Product("Wireless Mouse", new Money(79.99m, "USD"), 200, "Ergonomic wireless mouse with long battery life", "Electronics"),
                new Product("Mechanical Keyboard", new Money(149.99m, "USD"), 75, "RGB mechanical keyboard with cherry switches", "Electronics"),
                new Product("USB-C Hub", new Money(89.99m, "USD"), 100, "8-in-1 USB-C hub with 4K HDMI support", "Electronics"),
                new Product("Coffee Mug", new Money(19.99m, "USD"), 500, "Ceramic coffee mug with company logo", "Office Supplies"),
                new Product("Notebook Set", new Money(24.99m, "USD"), 300, "Set of 3 premium notebooks", "Office Supplies"),
            };

                context.Products.AddRange(products);
            }

            await context.SaveChangesAsync();
        }
    }
}
