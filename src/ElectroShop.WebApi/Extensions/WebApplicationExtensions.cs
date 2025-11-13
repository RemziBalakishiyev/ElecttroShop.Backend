using ElectroShop.WebApi.Middleware;

namespace ElectroShop.WebApi.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        // Exception Handling must be first
        app.UseExceptionHandling();

        // Swagger (Development və Production-da)
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ElectroShop API V1");
            c.RoutePrefix = "swagger";
        });

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}

