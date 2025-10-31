using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ParkingManagement.Application.Data.Queries.ParkingSpaces;
using ParkingManagement.Application.Infrastructure;
using ParkingManagement.Data.Queries.ParkingSpaces;
using ParkingManagement.Data.Repositories;
using ParkingManagement.Domain.Repositories;

namespace ParkingManagement.Data.Extensions;

/// <summary>
/// The service collection extensions for data services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds data services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddDataServices(this IServiceCollection services)
        => services
            .AddDbContext<ParkingDbContext>(options => options.UseInMemoryDatabase("CarParkingDatabase"))
            .AddScoped<IDataStore>(sp => sp.GetRequiredService<ParkingDbContext>())
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IParkingRepository, ParkingRepository>()
            .AddScoped<IOccupiedParkingSpacesDataQuery, OccupiedParkingSpacesDataQuery>();
}
