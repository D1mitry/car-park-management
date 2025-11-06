using DomainBase;

namespace ParkingManagement.Domain.Exceptions;

/// <summary>
/// The domain exception extensions.
/// </summary>
public static class DomainExceptionExtensions
{
    /// <summary>
    /// Throws exception for missing time out when leaving parking space.
    /// </summary>
    /// <param name="record">The parking record.</param>
    public static DomainValidationException BecauseTimeOutEmpty(this Parking record)
        => new("It is impossible to leave the parking space due to missing the departure time " +
            $"(VehicleReg: {record.VehicleReg}, VehicleType: {record.VehicleType}), ParkingSpace: {record.ParkingSpaceNumber}.")
        {
            Source = nameof(record)
        };

    /// <summary>
    /// Throws exception for missing basic charge when calculating charge.
    /// </summary>
    /// <param name="record">The parking record.</param>
    public static DomainValidationException BecauseBasicChargeMissing(this Parking record)
        => new("It is impossible to get the basic charge for this type of vehicle " +
            $"(VehicleReg: {record.VehicleReg}, VehicleType: {record.VehicleType}).")
        {
            Source = nameof(record)
        };

    internal static DomainValidationException BecauseEmptyRegistrationNumber(this string registration)
        => new($"Registration number cannot be empty.")
        {
            Source = nameof(registration)
        };

    internal static DomainValidationException BecauseInvalidRegistrationNumber(this string registration)
        => new($"Invalid registration number format: {registration}")
        {
            Source = nameof(registration)
        };

    internal static DomainConflictException BecauseVehicleAlreadyLeft(this Parking record)
        => new($"Vehicle with registration number {record.VehicleReg} is already left " +
            $"the parking space {record.ParkingSpaceNumber} at {record.TimeOut}.")
        {
            Source = nameof(record)
        };
}
