using DomainBase;
using ParkingManagement.Domain.Exceptions;
using ParkingManagement.Domain.Services;

namespace ParkingManagement.Domain;

/// <summary>
/// The parking aggregate root.
/// </summary>
public sealed class Parking
    : AggregateRoot<Guid>
{
    internal Parking(
        Guid id,
        VehicleRegistrationNumber vehicleReg,
        VehicleType vehicleType,
        ParkingSpaceNumber parkingSpaceNumber,
        DateTime timeIn,
        DateTime? timeOut,
        bool isParkingSpaceChargeApplied) : base(id)
    {
        VehicleReg = vehicleReg;
        VehicleType = vehicleType;
        ParkingSpaceNumber = parkingSpaceNumber;
        TimeIn = timeIn;
        TimeOut = timeOut;
        IsParkingSpaceChargeApplied = isParkingSpaceChargeApplied;
    }

    /// <summary>
    /// Gets the parking charge.
    /// </summary>
    public decimal Charge { get; private set; }

    /// <summary>
    /// Gets the parking space number.
    /// </summary>
    public ParkingSpaceNumber ParkingSpaceNumber { get; }

    /// <summary>
    /// Gets the time the vehicle entered the parking.
    /// </summary>
    public DateTime TimeIn { get; }

    /// <summary>
    /// Gets the time the vehicle left the parking.
    /// </summary>
    public DateTime? TimeOut { get; private set; }

    /// <summary>
    /// Gets the vehicle registration number.
    /// </summary>
    public VehicleRegistrationNumber VehicleReg { get; }

    /// <summary>
    /// Gets the vehicle type.
    /// </summary>
    public VehicleType VehicleType { get; }

    /// <summary>
    /// Gets a value indicating whether parking space charge is applied.
    /// </summary>
    public bool IsParkingSpaceChargeApplied { get; }

    /// <summary>
    /// Creates a new parking record.
    /// </summary>
    /// <param name="vehicleReg">The vehicle registration number.</param>
    /// <param name="vehicleType">The vehicle type.</param>
    /// <param name="parkingSpaceNumber">The parking space number.</param>
    /// <param name="timeIn">The time the vehicle entered the parking.</param>
    /// <param name="isParkingSpaceChargeApplied">Indicating whether parking space charge is applied.</param>
    /// <returns></returns>
    public static Parking Create(
        string vehicleReg,
        VehicleType vehicleType,
        int parkingSpaceNumber,
        DateTime timeIn,
        bool isParkingSpaceChargeApplied)
        => new(
            Guid.NewGuid(),
            VehicleRegistrationNumber.Create(vehicleReg),
            vehicleType,
            ParkingSpaceNumber.Create(parkingSpaceNumber),
            timeIn,
            null,
            isParkingSpaceChargeApplied);

    /// <summary>
    /// Exits the parking and calculates the charge.
    /// </summary>
    /// <param name="timeOut">The time the vehicle left the parking.</param>
    /// <param name="chargingService">The charging service to calculate the charge.</param>
    public void Exit(DateTime timeOut, IChargingService chargingService)
    {
        if (TimeOut.HasValue)
            throw this.BecauseVehicleAlreadyLeft();

        TimeOut = timeOut;
        Charge = chargingService.CalculateCharge(this);
    }
}
