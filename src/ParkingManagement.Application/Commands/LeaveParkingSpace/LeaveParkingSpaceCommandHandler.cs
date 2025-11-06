using FluentResults;
using MediatR;
using ParkingManagement.Application.Infrastructure.Errors;
using ParkingManagement.Application.Providers;
using ParkingManagement.Domain;
using ParkingManagement.Domain.Repositories;
using ParkingManagement.Domain.Services;

namespace ParkingManagement.Application.Commands.LeaveParkingSpace;

internal sealed class LeaveParkingSpaceCommandHandler(
    IDateTimeProvider dateTimeProvider,
    IParkingRepository repository,
    IChargingService chargingService)
    : IRequestHandler<LeaveParkingSpaceCommand, Result<LeaveParkingSpaceResponse>>
{
    public async Task<Result<LeaveParkingSpaceResponse>> Handle(LeaveParkingSpaceCommand request, CancellationToken cancellationToken)
    {
        var record = await repository.Find(VehicleRegistrationNumber.Create(request.VehicleReg), cancellationToken);

        if (record is null)
            return NotFoundError.BecauseVehicleIsNotParked(request.VehicleReg);

        record.Exit(dateTimeProvider.UtcNow, chargingService);

        if (!record.TimeOut.HasValue)
            return ValidationError.BecauseTimeOutEmpty(request.VehicleReg, record.VehicleType, record.ParkingSpaceNumber);

        await repository.Save(record, cancellationToken);

        return new LeaveParkingSpaceResponse(
            record.VehicleReg.Value,
            record.Charge,
            record.TimeIn,
            record.TimeOut.Value);
    }
}
