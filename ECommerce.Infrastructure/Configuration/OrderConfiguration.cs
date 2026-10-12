using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.ApplicationUserId)
                   .HasMaxLength(450)
                   .IsRequired();

            builder.Property(o => o.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(o => o.Subtotal).HasPrecision(18, 2);
            builder.Property(o => o.DiscountAmount).HasPrecision(18, 2);
            builder.Property(o => o.Total).HasPrecision(18, 2);

            builder.Property(o => o.PromotionCode).HasMaxLength(50);
            builder.Property(o => o.StripeSessionId).HasMaxLength(200);

            builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(o => o.ApplicationUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(o => o.Items)
                   .WithOne(i => i.Order)
                   .HasForeignKey(i => i.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(o => o.ApplicationUserId);
            builder.HasIndex(o => o.StripeSessionId);
        }
    }
}
