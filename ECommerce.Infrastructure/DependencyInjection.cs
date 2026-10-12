using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.DataAccess;
using ECommerce.Infrastructure.Identity;
using ECommerce.Infrastructure.Repositories;
using ECommerce.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Infrastructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            );

            //services.AddScoped<IRepository<Product>, Repository<Product>>();
            //services.AddScoped<IRepository<Brand>, Repository<Brand>>();
            //services.AddScoped<IRepository<Category>, Repository<Category>>();
            //services.AddScoped<IRepository<Cart>, Repository<Cart>>();
            //services.AddScoped<IRepository<ApplicationUserOTP>, Repository<ApplicationUserOTP>>();
            //services.AddScoped<IRepository<ProductColor>, Repository<ProductColor>>();
            //services.AddScoped<IRepository<Promotion>, Repository<Promotion>>();
            //services.AddScoped<IRepository<ProductSubImage>, Repository<ProductSubImage>>();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IPaymentService, StripePaymentService>();
            return services;
        }

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();
            return services;
        }
    }
}
