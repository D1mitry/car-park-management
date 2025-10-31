using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ParkingManagement.Application.Behaviors;
using ParkingManagement.Application.Configuration;
using ParkingManagement.Application.Providers;
using ParkingManagement.Application.Queries.ParkingSpaces;
using ParkingManagement.Application.Services;
using ParkingManagement.Domain.Services;
using System.Reflection;

namespace ParkingManagement.Application.Extensions;

/// <summary>
/// The service collection extensions for the application layer.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds application services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        => services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
            .AddSingleton<IDateTimeProvider, DateTimeProvider>()
            .AddSingleton<IChargingService, ChargingService>()
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>))
            .AddTransient<IRequestHandler<GetParkingSpacesQuery, ParkingSpacesQueryResponse>, GetParkingSpacesQueryHandler>();

    /// <summary>
    /// Adds application options to the service collection.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The application configuration,</param>
    public static IServiceCollection AddApplicationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ParkingOptions>(configuration.GetSection(ParkingOptions.SectionName));
        services.Configure<ChargingOptions>(configuration.GetSection(ChargingOptions.SectionName));

        services.AddSingleton<IConfigureOptions<ParkingOptions>, ParkingOptionsSetup>();

        return services;
    }
}
