using FluentResults;
using MediatR;
using Moq;
using ParkingManagement.Application.Commands.ParkVehicle;
using ParkingManagement.Application.Configuration;
using ParkingManagement.Application.Infrastructure.Errors;
using ParkingManagement.Application.Providers;
using ParkingManagement.Application.Queries.NextParkingSpace;
using ParkingManagement.Domain;
using ParkingManagement.Domain.Repositories;

namespace ParkingManagement.UnitTests.Commands;

public class ParkVehicleCommandHandlerShould
{
    private const string VehicleReg = "ABC123";
    private const int ParkingSpaceNumber = 1;
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

    private readonly Mock<IDateTimeProvider> dateTimeProvider = new();
    private readonly Mock<IMediator> mediator = new();
    private readonly Mock<IParkingRepository> parkingRepository = new();

    private readonly ParkVehicleCommandHandler commandHandler;

    public ParkVehicleCommandHandlerShould()
    {
        dateTimeProvider.Setup(p => p.UtcNow).Returns(Date);

        commandHandler = new(
            mediator.Object,
            dateTimeProvider.Object,
            ChargingOptions,
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

        mediator.Setup(q => q.Send(It.IsAny<GetNextParkingSpaceQuery>(), cancellationTokenSource.Token))
            .ReturnsAsync(new NextParkingSpaceResponse(1, 1));

        // Act
        Result<ParkVehicleResponse> result = await commandHandler.Handle(command, cancellationTokenSource.Token);

        // Assert
        parkingRepository.Verify(r => r.Save(It.IsAny<Parking>(), cancellationTokenSource.Token), Times.Once);

        Assert.Multiple(
            () => Assert.True(result.IsSuccess),
            () => Assert.Empty(result.Errors),
            () => Assert.Equal(VehicleReg, result.Value?.VehicleReg),
            () => Assert.Equal(ParkingSpaceNumber, result.Value?.SpaceNumber),
            () => Assert.Equal(Date, result.Value?.TimeIn));
    }

    [Theory]
    [InlineData(VehicleType.SmallCar)]
    [InlineData(VehicleType.MediumCar)]
    [InlineData(VehicleType.LargeCar)]
    public async Task ReturnConflictError_NoParkingSpaces(VehicleType vehicleType)
    {
        // Arrange
        ParkVehicleCommand command = new(VehicleReg, vehicleType);
        using CancellationTokenSource cancellationTokenSource = new();

        parkingRepository.Setup(r => r.Find(It.Is<VehicleRegistrationNumber>(n => n.Value == VehicleReg), cancellationTokenSource.Token))
            .ReturnsAsync(null as Parking);

        mediator.Setup(q => q.Send(It.IsAny<GetNextParkingSpaceQuery>(), cancellationTokenSource.Token))
            .ReturnsAsync(ConflictError.BecauseNoAvailableParkingSpaces());

        // Act
        Result<ParkVehicleResponse> result = await commandHandler.Handle(command, cancellationTokenSource.Token);

        // Assert
        Assert.Multiple(
            () => Assert.False(result.IsSuccess),
            () => Assert.Null(result.ValueOrDefault),
            () => Assert.All(result.Errors,
                error => Assert.Multiple(
                    () => Assert.IsType<ConflictError>(error),
                    () => Assert.StartsWith("No available parking spaces", error.Message))));
    }

    [Theory]
    [InlineData(VehicleType.SmallCar)]
    [InlineData(VehicleType.MediumCar)]
    [InlineData(VehicleType.LargeCar)]
    public async Task ReturnConflictError_VehicleAlreadyParked(VehicleType vehicleType)
    {
        // Arrange
        ParkVehicleCommand command = new(VehicleReg, vehicleType);
        using CancellationTokenSource cancellationTokenSource = new();

        parkingRepository.Setup(r => r.Find(It.Is<VehicleRegistrationNumber>(n => n.Value == VehicleReg), cancellationTokenSource.Token))
            .ReturnsAsync(Parking.Create(VehicleReg, vehicleType, 1, Date, false));

        // Act
        Result<ParkVehicleResponse> result = await commandHandler.Handle(command, cancellationTokenSource.Token);

        // Assert
        Assert.Multiple(
            () => Assert.False(result.IsSuccess),
            () => Assert.Null(result.ValueOrDefault),
            () => Assert.All(result.Errors,
                error => Assert.Multiple(
                    () => Assert.IsType<ConflictError>(error),
                    () => Assert.StartsWith($"Vehicle with registration number '{VehicleReg}' is already parked", error.Message))));
    }
}
