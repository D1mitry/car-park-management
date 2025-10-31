using MediatR;
using ParkingManagement.Application.Providers;
using ParkingManagement.Domain;
using ParkingManagement.Domain.Exceptions;
using ParkingManagement.Domain.Repositories;
using ParkingManagement.Domain.Services;

namespace ParkingManagement.Application.Commands.LeaveParkingSpace;

internal sealed class LeaveParkingSpaceCommandHandler(
    IDateTimeProvider dateTimeProvider,
    IParkingRepository repository,
    IChargingService chargingService)
    : IRequestHandler<LeaveParkingSpaceCommand, LeaveParkingSpaceResponse?>
{
    public async Task<LeaveParkingSpaceResponse?> Handle(LeaveParkingSpaceCommand request, CancellationToken cancellationToken)
    {
        var record = await repository.Find(VehicleRegistrationNumber.Create(request.VehicleReg), cancellationToken);

        if (record is null)
            return null;

        record.Exit(dateTimeProvider.UtcNow, chargingService);

        if (!record.TimeOut.HasValue)
            throw record.BecauseTimeOutEmpty();

        await repository.Save(record, cancellationToken);

        return new LeaveParkingSpaceResponse(
            record.VehicleReg.Value,
            record.Charge,
            record.TimeIn,
            record.TimeOut.Value);
    }
}
