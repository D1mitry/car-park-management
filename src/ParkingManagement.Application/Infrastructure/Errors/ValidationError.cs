using FluentResults;
using ParkingManagement.Domain;

namespace ParkingManagement.Application.Infrastructure.Errors;

/// <summary>
/// The validation error.
/// </summary>
public sealed class ValidationError : Error
{
    private ValidationError(string message)
        : base(message)
    {
    }

    internal static ValidationError BecauseTimeOutEmpty(string vehicleReg, VehicleType vehicleType, int parkingSpaceNumber)
        => new("It is impossible to leave the parking space due to missing the departure time " +
            $"(VehicleReg: {vehicleReg}, VehicleType: {vehicleType}), ParkingSpace: {parkingSpaceNumber}.");
}
