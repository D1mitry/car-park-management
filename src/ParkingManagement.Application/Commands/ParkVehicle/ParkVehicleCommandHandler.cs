using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using ParkingManagement.Application.Configuration;
using ParkingManagement.Application.Infrastructure.Errors;
using ParkingManagement.Application.Providers;
using ParkingManagement.Application.Queries.NextParkingSpace;
using ParkingManagement.Domain;
using ParkingManagement.Domain.Repositories;

namespace ParkingManagement.Application.Commands.ParkVehicle;

internal sealed class ParkVehicleCommandHandler(
    IMediator mediator,
    IDateTimeProvider dateTimeProvider,
    IOptionsSnapshot<ChargingOptions> chargingOptions,
    IParkingRepository repository)
    : IRequestHandler<ParkVehicleCommand, Result<ParkVehicleResponse>>
{
    public async Task<Result<ParkVehicleResponse>> Handle(ParkVehicleCommand request, CancellationToken cancellationToken)
    {
        Parking? record = await repository.Find(VehicleRegistrationNumber.Create(request.VehicleReg), cancellationToken);

        if (record is not null)
            return ConflictError.BecauseVehicleAlreadyParked(request.VehicleReg);

        var nextParkingSpace = await mediator.Send(new GetNextParkingSpaceQuery(), cancellationToken);

        if (nextParkingSpace.IsFailed)
            return nextParkingSpace.ToResult();

        record = Parking.Create(
            request.VehicleReg,
            request.VehicleType,
            nextParkingSpace.Value.NextSpace,
            dateTimeProvider.UtcNow,
            nextParkingSpace.Value.AvailableSpaces <= chargingOptions.Value.ParkingSpace.LastOf);

        await repository.Save(record, cancellationToken);

        return new ParkVehicleResponse(record.VehicleReg, record.ParkingSpaceNumber, record.TimeIn);
    }
}
