using ElectroShop.WebApi.Middleware;

namespace ElectroShop.WebApi.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        // Exception Handling must be first
        app.UseExceptionHandling();

        // Swagger (Development only)
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ElectroShop API V1");
                c.RoutePrefix = "swagger";
            });
        }

        if (!app.Environment.IsProduction())
        {
            app.UseHttpsRedirection();
        }

        app.UseCors("Frontend");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}

