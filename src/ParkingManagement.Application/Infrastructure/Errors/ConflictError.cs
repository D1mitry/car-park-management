using FluentResults;

namespace ParkingManagement.Application.Infrastructure.Errors;

/// <summary>
/// The conflict error.
/// </summary>
public sealed class ConflictError : Error
{
    private ConflictError(string message)
        : base(message)
    {
    }

    internal static ConflictError BecauseVehicleAlreadyParked(string vehicleReg)
        => new($"Vehicle with registration number '{vehicleReg}' is already parked.");

    internal static ConflictError BecauseNoAvailableParkingSpaces()
        => new("No available parking spaces.");
}
