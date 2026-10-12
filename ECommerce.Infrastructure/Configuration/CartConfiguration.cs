using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Configuration
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {

            builder.HasKey(e => e.Id);

            builder.HasOne<ApplicationUser>()
           .WithMany()
           .HasForeignKey(c => c.ApplicationUserId)
           .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(c => new { c.ApplicationUserId, c.ProductId })
                .IsUnique();
            builder.HasOne(c => c.Product)
                .WithMany()
                .HasForeignKey(c => c.ProductId);
        }
    }
}
