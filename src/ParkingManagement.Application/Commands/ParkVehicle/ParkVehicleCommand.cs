using FluentResults;
using ParkingManagement.Domain;

namespace ParkingManagement.Application.Commands.ParkVehicle;

/// <summary>
/// Represents a command to park a vehicle in the parking space.
/// </summary>
/// <param name="VehicleReg">The registration number of the vehicle</param>
/// <param name="VehicleType">The type of the vehicle for which the parking space is being reserved.</param>
public sealed record ParkVehicleCommand(string VehicleReg, VehicleType VehicleType) : IApplicationCommand<Result<ParkVehicleResponse>>;
