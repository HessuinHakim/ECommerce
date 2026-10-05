using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Application.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Infrastructure.Identity
{
    public class IdentitySeeder
    {


        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var configuration = services.GetRequiredService<IConfiguration>();

            foreach (var role in new[] { Roles.Admin, Roles.Customer })
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            var AdminEmail = configuration["AdminEmail"];
            var password = configuration["AdminPassword"];

            if (string.IsNullOrWhiteSpace(AdminEmail) || string.IsNullOrWhiteSpace(password))
                return;

            if (await userManager.FindByEmailAsync(AdminEmail) is not null)
                return;

           
                var admin = new ApplicationUser
                {
                    UserName = AdminEmail,
                    Email = AdminEmail,
                    FullName = "Admin",
                    EmailConfirmed = true
                };
            var result = await userManager.CreateAsync(admin, password);
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    "Failed to create admin: " + string.Join(", ", result.Errors.Select(e => e.Description)));

            await userManager.AddToRoleAsync(admin, Roles.Admin);


            
        }
    }
}
