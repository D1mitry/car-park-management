using DomainBase;
using ParkingManagement.Application.Configuration;
using ParkingManagement.Application.Services;
using ParkingManagement.Domain;

namespace ParkingManagement.UnitTests.Services;

public sealed class CalculateChargeShould
{
    private static readonly VehicleRegistrationNumber VehicleReg = VehicleRegistrationNumber.Create("ABC123");
    private static readonly ParkingSpaceNumber ParkingSpaceNumber = ParkingSpaceNumber.Create(1);
    private static readonly DateTime Date = new(2025, 10, 15, 10, 50, 00, DateTimeKind.Utc);
    private static readonly TestOptionsSnapshot<ChargingOptions> ChargingOptions = new(new ChargingOptions
    {
        Basic = new Dictionary<VehicleType, decimal>
            {
                { VehicleType.SmallCar, 0.1m },
                { VehicleType.MediumCar, 0.2m },
                { VehicleType.LargeCar, 0.4m }
            },
        Additional = new ChargingOptions.AdditionalCharge(5, 1m),
        ParkingSpace = new ChargingOptions.ParkingSpaceCharge(1, 5m)
    });

    private ChargingService chargingService = new(ChargingOptions);

    [Theory]
    [InlineData(VehicleType.SmallCar, 1, 0)]
    [InlineData(VehicleType.SmallCar, 10, 0)]
    [InlineData(VehicleType.SmallCar, 59, 0)]
    [InlineData(VehicleType.SmallCar, 60, 0.1)]
    [InlineData(VehicleType.SmallCar, 120, 0.2)]
    [InlineData(VehicleType.SmallCar, 180, 0.3)]
    [InlineData(VehicleType.SmallCar, 240, 0.4)]
    [InlineData(VehicleType.SmallCar, 300, 1.5)]
    [InlineData(VehicleType.SmallCar, 360, 1.6)]
    [InlineData(VehicleType.SmallCar, 420, 1.7)]
    [InlineData(VehicleType.SmallCar, 600, 3.0)]
    [InlineData(VehicleType.SmallCar, 800, 3.3)]

    [InlineData(VehicleType.MediumCar, 1, 0)]
    [InlineData(VehicleType.MediumCar, 10, 0)]
    [InlineData(VehicleType.MediumCar, 59, 0)]
    [InlineData(VehicleType.MediumCar, 60, 0.2)]
    [InlineData(VehicleType.MediumCar, 120, 0.4)]
    [InlineData(VehicleType.MediumCar, 180, 0.6)]
    [InlineData(VehicleType.MediumCar, 240, 0.8)]
    [InlineData(VehicleType.MediumCar, 300, 2.0)]
    [InlineData(VehicleType.MediumCar, 360, 2.2)]
    [InlineData(VehicleType.MediumCar, 420, 2.4)]
    [InlineData(VehicleType.MediumCar, 600, 4.0)]
    [InlineData(VehicleType.MediumCar, 800, 4.6)]

    [InlineData(VehicleType.LargeCar, 1, 0)]
    [InlineData(VehicleType.LargeCar, 10, 0)]
    [InlineData(VehicleType.LargeCar, 59, 0)]
    [InlineData(VehicleType.LargeCar, 60, 0.4)]
    [InlineData(VehicleType.LargeCar, 120, 0.8)]
    [InlineData(VehicleType.LargeCar, 180, 1.2)]
    [InlineData(VehicleType.LargeCar, 240, 1.6)]
    [InlineData(VehicleType.LargeCar, 300, 3.0)]
    [InlineData(VehicleType.LargeCar, 360, 3.4)]
    [InlineData(VehicleType.LargeCar, 420, 3.8)]
    [InlineData(VehicleType.LargeCar, 600, 6.0)]
    [InlineData(VehicleType.LargeCar, 800, 7.2)]

    public void CalculateExpectedCharge(VehicleType vehicleType, int parkingTimeSeconds, decimal expectedCharge)
    {
        // Arrange
        Parking record = GetParkingRecord(
            VehicleReg,
            vehicleType,
            ParkingSpaceNumber,
            Date,
            Date.AddSeconds(parkingTimeSeconds),
            false);

        // Act
        decimal result = chargingService.CalculateCharge(record);

        Assert.Equal(expectedCharge, result);
    }

    [Theory]
    [InlineData(VehicleType.SmallCar, 300, 6.5)]
    [InlineData(VehicleType.MediumCar, 300, 7.0)]
    [InlineData(VehicleType.LargeCar, 300, 8.0)]
    public void CalculateExpectedParkingSpaceCharge(VehicleType vehicleType, int parkingTimeSeconds, decimal expectedCharge)
    {
        // Arrange
        Parking record = GetParkingRecord(
            VehicleReg,
            vehicleType,
            ParkingSpaceNumber,
            Date,
            Date.AddSeconds(parkingTimeSeconds),
            true);

        // Act
        decimal result = chargingService.CalculateCharge(record);

        Assert.Equal(expectedCharge, result);
    }

    [Theory]
    [InlineData(VehicleType.SmallCar)]
    [InlineData(VehicleType.MediumCar)]
    [InlineData(VehicleType.LargeCar)]
    public void ThrowDomainValidationException_BasicChangeMissing(VehicleType vehicleType)
    {
        // Arrange
        Parking record = GetParkingRecord(
            VehicleReg,
            vehicleType,
            ParkingSpaceNumber,
            Date,
            Date.AddMinutes(6),
            false);

        chargingService = new(new TestOptionsSnapshot<ChargingOptions>(new ChargingOptions()));

        // Act / Assert
        var exception = Assert.Throws<DomainValidationException>(() => chargingService.CalculateCharge(record));
        Assert.StartsWith("It is impossible to get the basic charge for this type of vehicle", exception.Message);
    }

    [Theory]
    [InlineData(VehicleType.SmallCar)]
    [InlineData(VehicleType.MediumCar)]
    [InlineData(VehicleType.LargeCar)]
    public void ThrowDomainValidationException_TimeOutIsNull(VehicleType vehicleType)
    {
        // Arrange
        Parking record = GetParkingRecord(
            VehicleReg,
            vehicleType,
            ParkingSpaceNumber,
            Date,
            null,
            false);

        // Act / Assert
        var exception = Assert.Throws<DomainValidationException>(() => chargingService.CalculateCharge(record));
        Assert.StartsWith("It is impossible to leave the parking space due to missing the departure time", exception.Message);
    }

    private static Parking GetParkingRecord(
        VehicleRegistrationNumber vehicleReg,
        VehicleType vehicleType,
        ParkingSpaceNumber parkingSpaceNumber,
        DateTime timeIn,
        DateTime? timeOut,
        bool isParkingSpaceChargeApplied)
            => new(Guid.NewGuid(), vehicleReg, vehicleType, parkingSpaceNumber, timeIn, timeOut, isParkingSpaceChargeApplied);
}