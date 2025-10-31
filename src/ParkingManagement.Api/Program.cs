using ParkingManagement.Api.Extensions;

await WebApplication.CreateBuilder(args)
    .ConfigureWebHost()
    .Build()
    .ConfigureWebApplication()
    .RunAsync();
