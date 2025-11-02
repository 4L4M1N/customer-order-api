using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace CustomerOrderManagement.Api.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseSwaggerDocumentation(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer-Order Management API v1");
                    c.RoutePrefix = string.Empty;
                });
            }

            return app;
        }
    }
}