using ParkingManagement.Api.Configuration;
using ParkingManagement.Api.Filters;

namespace ParkingManagement.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFluentResultsErrors(this IServiceCollection services, Action<FluentResultsErrorOptions>? configure = null)
    {
        var options = new FluentResultsErrorOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<FluentResultsTransformer>();

        return services;
    }

    public static IServiceCollection AddExceptionsMapping(this IServiceCollection services, Action<ExceptionErrorOptions>? configure = null)
    {
        var options = new ExceptionErrorOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);

        return services;
    }
}
