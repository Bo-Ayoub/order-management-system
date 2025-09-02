using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Infrastructure.Data.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.Id)
                .ValueGeneratedNever();

            builder.Property(oi => oi.Quantity)
                .IsRequired();

            // Value Object mapping for UnitPrice
            builder.OwnsOne(oi => oi.UnitPrice, price =>
            {
                price.Property(m => m.Amount)
                    .HasColumnName("UnitPrice")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                price.Property(m => m.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            builder.Property(oi => oi.CreatedAt)
                .IsRequired();

            builder.Property(oi => oi.UpdatedAt);

            // Foreign key to Product
            builder.Property(oi => oi.ProductId)
                .IsRequired();

            builder.HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Shadow property for Order foreign key
            builder.Property<Guid>("OrderId")
                .IsRequired();

            // Ignore computed properties
            builder.Ignore(oi => oi.TotalPrice);
            builder.Ignore(oi => oi.DomainEvents);

            // Indexes
            builder.HasIndex(oi => oi.ProductId);
            builder.HasIndex("OrderId");
        }
    }
}
