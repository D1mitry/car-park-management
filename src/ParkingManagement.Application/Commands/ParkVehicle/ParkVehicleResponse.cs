namespace ParkingManagement.Application.Commands.ParkVehicle;

/// <summary>
/// Represents the response received after vehicle is parked at the parking sspace.
/// </summary>
/// <param name="VehicleReg">The registration number of the vehicle for which the parking space is reserved.</param>
/// <param name="SpaceNumber">The number of the parking space that has been reserved.</param>
/// <param name="TimeIn">The date and time when the vehicle enters the reserved parking space.</param>
public sealed record ParkVehicleResponse(string VehicleReg, int SpaceNumber, DateTime TimeIn);
