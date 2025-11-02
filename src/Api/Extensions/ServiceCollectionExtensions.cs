using System;
using System.IO;
using System.Reflection;
using Application.Commands.Product;
using Application.Queries.Customer;
using Application.Queries.Order;
using Application.Queries.Product;
using CustomerOrderManagement.Application.Commands.Customer;
using CustomerOrderManagement.Application.Commands.Order;
using CustomerOrderManagement.Application.Commands.ShoppingCart;
using CustomerOrderManagement.Application.Queries.ShoppingCart;
using CustomerOrderManagement.Infrastructure.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace CustomerOrderManagement.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOrderManagementServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<CustomerCommandHandler>();
            services.AddScoped<OrderCommandHandler>();
            services.AddScoped<ProductCommandHandler>();
            services.AddScoped<ShoppingCartCommandHandler>();

            services.AddScoped<CustomerQueryHandler>();
            services.AddScoped<OrderQueryHandler>();
            services.AddScoped<ProductQueryHandler>();
            services.AddScoped<ShoppingCartQueryHandler>();

            return services;
        }

        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Customer-Order Management API",
                    Version = "v1",
                    Description = "A RESTful API for managing customers and orders using DDD and CQRS patterns",
                    Contact = new OpenApiContact
                    {
                        Name = "Dev"
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            return services;
        }
    }
}