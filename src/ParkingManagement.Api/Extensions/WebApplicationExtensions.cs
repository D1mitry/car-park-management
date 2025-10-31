namespace ParkingManagement.Api.Extensions;

internal static class WebApplicationExtensions
{
    public static WebApplication ConfigureWebApplication(this WebApplication app)
    {
        app.UseExceptionHandler();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger()
                .UseSwaggerUI();

            app.MapGet("/", () => Results.Redirect("/swagger"))
                .ExcludeFromDescription();
        }
        else
        {
            app.MapGet("/", () => Results.Redirect("/hc"))
                .ExcludeFromDescription();
        }

        app.UseHttpsRedirection()
            .UseHsts();

        app.MapHealthChecks("/hc");
        app.MapControllers();

        return app;
    }
}
