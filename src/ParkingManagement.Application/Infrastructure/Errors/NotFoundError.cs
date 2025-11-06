using FluentResults;

namespace ParkingManagement.Application.Infrastructure.Errors;

/// <summary>
/// The error indicating that a requested resource was not found.
/// </summary>
public sealed class NotFoundError : Error
{
    private NotFoundError(string message)
        : base(message)
    {
    }

    internal static NotFoundError BecauseVehicleIsNotParked(string vehicleReg)
        => new($"Vehicle with registration number '{vehicleReg}' has not been found in the parking.");
}
