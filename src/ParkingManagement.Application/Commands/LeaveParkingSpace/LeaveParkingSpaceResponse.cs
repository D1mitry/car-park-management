namespace ParkingManagement.Application.Commands.LeaveParkingSpace;

/// <summary>
/// Represents the response received after a vehicle exits a parking space.
/// </summary>
/// <param name="VehicleReg">The registration number of the vehicle that exited the parking space.</param>
/// <param name="VehicleCharge">The total charge incurred by the vehicle for the parking duration.</param>
/// <param name="TimeIn">The date and time when the vehicle entered the parking space.</param>
/// <param name="TimeOut">The date and time when the vehicle exited the parking space.</param>
public sealed record LeaveParkingSpaceResponse(string VehicleReg, decimal VehicleCharge, DateTime TimeIn, DateTime TimeOut);