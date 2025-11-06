using Moq;
using ParkingManagement.Domain;
using ParkingManagement.Domain.Services;

namespace ParkingManagement.UnitTests.Domain;

public sealed class ParkingShould
{
    private const string VehicleReg = "ABC123";
    private const int ParkingSpaceNumber = 5;
    private static readonly DateTime Date = new(2025, 10, 15, 10, 50, 00, DateTimeKind.Utc);

    [Theory]
    [InlineData(VehicleType.SmallCar, false)]
    [InlineData(VehicleType.MediumCar, false)]
    [InlineData(VehicleType.LargeCar, false)]
    [InlineData(VehicleType.SmallCar, true)]
    [InlineData(VehicleType.MediumCar, true)]
    [InlineData(VehicleType.LargeCar, true)]
    public void ReturnExpectedParkingRecord_Create(VehicleType vehicleType, bool isParkingSpaceChargeApplied)
    {
        // Act
        var record = Parking.Create(VehicleReg, vehicleType, ParkingSpaceNumber, Date, isParkingSpaceChargeApplied);

        // Assert
        Assert.Multiple(
            () => Assert.Equal(VehicleReg, record.VehicleReg.Value),
            () => Assert.Equal(vehicleType, record.VehicleType),
            () => Assert.Equal(ParkingSpaceNumber, record.ParkingSpaceNumber.Value),
            () => Assert.Equal(Date, record.TimeIn),
            () => Assert.Null(record.TimeOut),
            () => Assert.Equal(0m, record.Charge));
    }

    [Theory]
    [InlineData(VehicleType.SmallCar, 9, false)]
    [InlineData(VehicleType.MediumCar, 12, false)]
    [InlineData(VehicleType.LargeCar, 18, false)]
    [InlineData(VehicleType.SmallCar, 9, true)]
    [InlineData(VehicleType.MediumCar, 12, true)]
    [InlineData(VehicleType.LargeCar, 18, true)]
    public void ReturnExpectedParkingRecord_Exit(VehicleType vehicleType, decimal charge, bool isParkingSpaceChargeApplied)
    {
        // Arrange
        var record = Parking.Create(VehicleReg, vehicleType, ParkingSpaceNumber, Date, isParkingSpaceChargeApplied);
        var timeOut = Date.AddMinutes(30);

        Mock<IChargingService> chargingService = new();

        chargingService.Setup(s => s.CalculateCharge(It.IsAny<Parking>()))
            .Returns(charge);

        // Act
        record.Exit(timeOut, chargingService.Object);

        // Assert
        Assert.Multiple(
            () => Assert.Equal(VehicleReg, record.VehicleReg.Value),
            () => Assert.Equal(vehicleType, record.VehicleType),
            () => Assert.Equal(ParkingSpaceNumber, record.ParkingSpaceNumber.Value),
            () => Assert.Equal(Date, record.TimeIn),
            () => Assert.Equal(timeOut, record.TimeOut),
            () => Assert.Equal(charge, record.Charge));
    }
}
