using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Configuration
{
    public class ProductSubImageConfiguration : IEntityTypeConfiguration<ProductSubImage>
    {
        public void Configure( EntityTypeBuilder<ProductSubImage> builder)
        {
            builder.HasKey(x => new
            {
                x.ProductId,
                x.Img
            });

            builder.Property(x => x.Img)
                .IsRequired();

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
