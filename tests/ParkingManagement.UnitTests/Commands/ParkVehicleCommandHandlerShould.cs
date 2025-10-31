using Microsoft.Extensions.Options;
using Moq;
using ParkingManagement.Application.Commands.ParkVehicle;
using ParkingManagement.Application.Configuration;
using ParkingManagement.Application.Data.Queries.ParkingSpaces;
using ParkingManagement.Application.Exceptions;
using ParkingManagement.Application.Providers;
using ParkingManagement.Domain;
using ParkingManagement.Domain.Repositories;

namespace ParkingManagement.UnitTests.Commands;

public class ParkVehicleCommandHandlerShould
{
    private const string VehicleReg = "ABC123";
    private const int ParkingSpaceNumber = 1;
    private static readonly DateTime Date = new(2025, 10, 15, 10, 50, 00, DateTimeKind.Utc);
    private static readonly ParkingOptions ParkingOptions = new()
    {
        TotalSpaces = 1,
        Spaces = [ParkingSpaceNumber],
    };

    private readonly Mock<IDateTimeProvider> dateTimeProvider = new();
    private readonly Mock<IOccupiedParkingSpacesDataQuery> occupiedParkingSpacesDataQuery = new();
    private readonly Mock<IParkingRepository> parkingRepository = new();

    private readonly ParkVehicleCommandHandler commandHandler;

    public ParkVehicleCommandHandlerShould()
    {
        dateTimeProvider.Setup(p => p.UtcNow).Returns(Date);

        commandHandler = new(
            dateTimeProvider.Object,
            occupiedParkingSpacesDataQuery.Object,
            Options.Create(ParkingOptions),
            parkingRepository.Object);
    }

    [Theory]
    [InlineData(VehicleType.SmallCar)]
    [InlineData(VehicleType.MediumCar)]
    [InlineData(VehicleType.LargeCar)]
    public async Task ReturnExpectedResult(VehicleType vehicleType)
    {
        // Arrange
        ParkVehicleCommand command = new(VehicleReg, vehicleType);
        using CancellationTokenSource cancellationTokenSource = new();

        parkingRepository.Setup(r => r.Find(It.Is<VehicleRegistrationNumber>(n => n.Value == VehicleReg), cancellationTokenSource.Token))
            .ReturnsAsync(null as Parking);

        occupiedParkingSpacesDataQuery.Setup(q => q.Get()).ReturnsAsync([]);

        // Act
        ParkVehicleResponse result = await commandHandler.Handle(command, cancellationTokenSource.Token);

        // Assert
        parkingRepository.Verify(r => r.Save(It.IsAny<Parking>(), cancellationTokenSource.Token), Times.Once);

        Assert.Multiple(
            () => Assert.Equal(VehicleReg, result.VehicleReg),
            () => Assert.Equal(ParkingSpaceNumber, result.SpaceNumber),
            () => Assert.Equal(Date, result.TimeIn));
    }

    [Theory]
    [InlineData(VehicleType.SmallCar)]
    [InlineData(VehicleType.MediumCar)]
    [InlineData(VehicleType.LargeCar)]
    public async Task ThrowConflictException_NoParkingSpaces(VehicleType vehicleType)
    {
        // Arrange
        ParkVehicleCommand command = new(VehicleReg, vehicleType);
        using CancellationTokenSource cancellationTokenSource = new();

        parkingRepository.Setup(r => r.Find(It.Is<VehicleRegistrationNumber>(n => n.Value == VehicleReg), cancellationTokenSource.Token))
            .ReturnsAsync(null as Parking);

        occupiedParkingSpacesDataQuery.Setup(q => q.Get()).ReturnsAsync([1]);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ConflictException>(() => commandHandler.Handle(command, cancellationTokenSource.Token));
        Assert.StartsWith("No available parking spaces", exception.Message);
    }

    [Theory]
    [InlineData(VehicleType.SmallCar)]
    [InlineData(VehicleType.MediumCar)]
    [InlineData(VehicleType.LargeCar)]
    public async Task ThrowConflictException_VehicleAlreadyParked(VehicleType vehicleType)
    {
        // Arrange
        ParkVehicleCommand command = new(VehicleReg, vehicleType);
        using CancellationTokenSource cancellationTokenSource = new();

        parkingRepository.Setup(r => r.Find(It.Is<VehicleRegistrationNumber>(n => n.Value == VehicleReg), cancellationTokenSource.Token))
            .ReturnsAsync(Parking.Create(VehicleReg, vehicleType, 1, Date));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ConflictException>(() => commandHandler.Handle(command, cancellationTokenSource.Token));
        Assert.StartsWith($"Vehicle with registration number '{VehicleReg}' is already parked", exception.Message);
    }
}
