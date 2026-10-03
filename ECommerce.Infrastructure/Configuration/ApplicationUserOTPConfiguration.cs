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

            builder.Property(x => x.OTP)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ValidTo)
                .IsRequired();

            builder.Property(x => x.IsValid)
                .IsRequired();

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
