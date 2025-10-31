using Microsoft.Extensions.Options;
using ParkingManagement.Api.Configuration;
using ParkingManagement.Application.Extensions;
using ParkingManagement.Data.Extensions;
using System.Collections.Immutable;
using System.Reflection;

namespace ParkingManagement.Api.Extensions;

internal static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder ConfigureWebHost(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddHealthChecks();

        builder.Services
            .AddProblemDetails()
            .AddSingleton<IConfigureOptions<ProblemDetailsOptions>, ProblemDetailsSetup>()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Car Parking Management API",
                    Version = "v1"
                });

                var xmlDocFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlDocPath = Path.Combine(AppContext.BaseDirectory, xmlDocFile);
                options.IncludeXmlComments(xmlDocPath);

                ImmutableList<Assembly> referencedAssemblies =
                [
                    typeof(Application.Extensions.ServiceCollectionExtensions).Assembly,
                    typeof(Domain.VehicleType).Assembly
                ];

                referencedAssemblies.ForEach(a =>
                {
                    xmlDocPath = Path.Combine(AppContext.BaseDirectory, $"{a.GetName().Name}.xml");
                    if (File.Exists(xmlDocPath))
                        options.IncludeXmlComments(xmlDocPath);
                });

            })
            .AddApplicationServices()
            .AddApplicationOptions(builder.Configuration)
            .AddDataServices();

        return builder;
    }
}
