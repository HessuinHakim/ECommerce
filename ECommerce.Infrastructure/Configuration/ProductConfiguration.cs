using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Configuration
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {

        public void Configure(EntityTypeBuilder <Product> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e=> e.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(e => e.Price)
                .HasPrecision(18, 2);

            builder.Property(e => e.Discount)
                .HasPrecision(18, 2);

            builder.Property(e => e.Rate)
                .HasPrecision(3, 2);



            // Category relationship

            builder.HasOne(e=>e.Category)
                .WithMany()
                .HasForeignKey(e=> e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            // Brand Relationship

            builder.HasOne(e=> e.Brand)
                .WithMany()
                .HasForeignKey(e => e.BrandId)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
