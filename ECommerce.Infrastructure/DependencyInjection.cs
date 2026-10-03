using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.DataAccess;
using ECommerce.Infrastructure.Repositories;
using ECommerce.Application.Services;
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

            return services;
        }

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IBrandService, BrandService>();
            return services;
        }
    }
}
