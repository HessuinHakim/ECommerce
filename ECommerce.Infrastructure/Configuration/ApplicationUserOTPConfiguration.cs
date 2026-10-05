using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Configuration
{
    public class ApplicationUserOTPConfiguration : IEntityTypeConfiguration<ApplicationUserOTP>
    {
        public void Configure(
               EntityTypeBuilder<ApplicationUserOTP> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OtpHash)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.CreatedAt)
                .IsRequired();
            builder.Property(x => x.Purpose)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.ValidTo)
                .IsRequired();

            builder.Property(x => x.IsUsed)
                .IsRequired();

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);
            
        }
    }
}
