using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Configuration
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.ProductName)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(i => i.UnitPrice).HasPrecision(18, 2);

            builder.HasOne<Product>()
                   .WithMany()
                   .HasForeignKey(i => i.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(i => i.OrderId);
        }
    }
}
