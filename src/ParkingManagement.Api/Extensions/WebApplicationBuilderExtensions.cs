using DomainBase;
using Microsoft.Extensions.Options;
using ParkingManagement.Api.Configuration;
using ParkingManagement.Api.Filters;
using ParkingManagement.Application.Extensions;
using ParkingManagement.Application.Infrastructure.Errors;
using ParkingManagement.Data.Extensions;
using System.Collections.Immutable;
using System.Reflection;

namespace ParkingManagement.Api.Extensions;

internal static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder ConfigureWebHost(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(options =>
                options.Filters.Add<FluentResultsAsyncFilter>());
        builder.Services.AddHealthChecks();

        builder.Services
            .AddProblemDetails()
            .AddExceptionsMapping(options =>
            {
                options.MapException<DomainValidationException>(StatusCodes.Status400BadRequest, "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.1");
                options.MapException<DomainConflictException>(StatusCodes.Status409Conflict, "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.10");
            })
            .AddFluentResultsErrors(options =>
            {
                options.MapError<ValidationError>(StatusCodes.Status400BadRequest, "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.1");
                options.MapError<ConflictError>(StatusCodes.Status409Conflict, "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.10");
                options.MapError<NotFoundError>(StatusCodes.Status404NotFound, "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5");
            })
            .AddSingleton<IConfigureOptions<ProblemDetailsOptions>, ProblemDetailsConfiguration>()
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

                options.OperationFilter<FluentResultOperationFilter>();
                options.SchemaFilter<FluentResultSchemaFilter>();
                options.DocumentFilter<FluentResultSchemaFilter>();
            })
            .AddApplicationServices()
            .AddApplicationOptions(builder.Configuration)
            .AddDataServices();

        return builder;
    }
}
