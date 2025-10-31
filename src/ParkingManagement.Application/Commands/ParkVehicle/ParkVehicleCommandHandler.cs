using MediatR;
using Microsoft.Extensions.Options;
using ParkingManagement.Application.Configuration;
using ParkingManagement.Application.Data.Queries.ParkingSpaces;
using ParkingManagement.Application.Exceptions;
using ParkingManagement.Application.Providers;
using ParkingManagement.Domain;
using ParkingManagement.Domain.Repositories;

namespace ParkingManagement.Application.Commands.ParkVehicle;

internal sealed class ParkVehicleCommandHandler(
    IDateTimeProvider dateTimeProvider,
    IOccupiedParkingSpacesDataQuery query,
    IOptions<ParkingOptions> parkingOptions,
    IParkingRepository repository)
    : IRequestHandler<ParkVehicleCommand, ParkVehicleResponse>
{
    public async Task<ParkVehicleResponse> Handle(ParkVehicleCommand request, CancellationToken cancellationToken)
    {
        Parking? record = await repository.Find(VehicleRegistrationNumber.Create(request.VehicleReg), cancellationToken);

        if (record is not null)
            throw ConflictException.BecauseVehicleAlreadyParked(request.VehicleReg);

        var nextSpace = parkingOptions.Value.Spaces.Except(await query.Get()).FirstOrDefault();

        if (nextSpace == 0)
            throw ConflictException.BecauseNoAvailableParkingSpaces();

        record = Parking.Create(
            request.VehicleReg,
            request.VehicleType,
            nextSpace,
            dateTimeProvider.UtcNow);

        await repository.Save(record, cancellationToken);

        return new(record.VehicleReg, record.ParkingSpaceNumber, record.TimeIn);
    }
}
