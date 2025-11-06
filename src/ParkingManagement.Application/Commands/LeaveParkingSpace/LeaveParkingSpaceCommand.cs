using FluentResults;

namespace ParkingManagement.Application.Commands.LeaveParkingSpace;

/// <summary>
/// Represents a command to exit a parking space for a vehicle.
/// </summary>
/// <param name="VehicleReg">The registration number of the vehicle exiting the parking space.</param>
public sealed record LeaveParkingSpaceCommand(string VehicleReg) : IApplicationCommand<Result<LeaveParkingSpaceResponse>>;
