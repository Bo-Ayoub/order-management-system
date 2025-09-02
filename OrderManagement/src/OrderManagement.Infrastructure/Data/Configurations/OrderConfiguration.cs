using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .ValueGeneratedNever();

            builder.Property(o => o.OrderNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.Status)
                .IsRequired()
                .HasConversion<string>() // Store enum as string in database
                .HasMaxLength(20);

            builder.Property(o => o.OrderDate)
                .IsRequired();

            builder.Property(o => o.ShippedDate);

            builder.Property(o => o.DeliveredDate);

            builder.Property(o => o.ShippingAddress)
                .HasMaxLength(500);

            builder.Property(o => o.Notes)
                .HasMaxLength(1000);

            builder.Property(o => o.CreatedAt)
                .IsRequired();

            builder.Property(o => o.UpdatedAt);

            // Foreign key to Customer
            builder.Property(o => o.CustomerId)
                .IsRequired();

            builder.HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-many relationship with OrderItems
            builder.HasMany(o => o.OrderItems)
                .WithOne()
                .HasForeignKey("OrderId") // Shadow property
                .OnDelete(DeleteBehavior.Cascade); // Delete order items when order is deleted

            // Ignore computed properties
            builder.Ignore(o => o.TotalAmount);
            builder.Ignore(o => o.TotalItems);
            builder.Ignore(o => o.DomainEvents);

            // Indexes
            builder.HasIndex(o => o.OrderNumber)
                .IsUnique();
            builder.HasIndex(o => o.CustomerId);
            builder.HasIndex(o => o.Status);
            builder.HasIndex(o => o.OrderDate);
            builder.HasIndex(o => new { o.Status, o.OrderDate });
        }
    }

}
