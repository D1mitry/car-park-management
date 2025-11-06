using Microsoft.EntityFrameworkCore;
using ParkingManagement.Data.Entities;
using ParkingManagement.Domain;
using ParkingManagement.Domain.Repositories;

namespace ParkingManagement.Data.Repositories;

internal sealed class ParkingRepository(ParkingDbContext context) : IParkingRepository
{
    public async Task<Parking?> Find(VehicleRegistrationNumber vehicleReg, CancellationToken cancellationToken)
    {
        ParkingEntity? entity = GetParkingEntity(vehicleReg) ?? await FetchParkingEntity(vehicleReg, cancellationToken);

        if (entity is null)
            return null;

        return new(
            entity.Id,
            VehicleRegistrationNumber.Create(entity.VehicleReg),
            entity.VehicleType,
            ParkingSpaceNumber.Create(entity.ParkingSpaceNumber),
            entity.TimeIn,
            entity.TimeOut,
            entity.IsParkingSpaceChargeApplied);
    }

    public async Task Save(Parking record, CancellationToken cancellationToken)
    {
        ParkingEntity? entity = GetParkingEntity(record.VehicleReg);

        if (entity is null)
        {
            entity = new ParkingEntity
            {
                Id = record.Id,
                ParkingSpaceNumber = record.ParkingSpaceNumber.Value,
                VehicleReg = record.VehicleReg.Value,
                VehicleType = record.VehicleType,
                TimeIn = record.TimeIn,
                IsParkingSpaceChargeApplied = record.IsParkingSpaceChargeApplied,
            };

            await context.Set<ParkingEntity>().AddAsync(entity, cancellationToken);
        }

        entity.TimeOut = record.TimeOut;
    }

    private ParkingEntity? GetParkingEntity(VehicleRegistrationNumber vehicleReg)
        => context.Set<ParkingEntity>().Local.FindEntry(nameof(ParkingEntity.VehicleReg), vehicleReg.Value)?.Entity;

    private Task<ParkingEntity?> FetchParkingEntity(VehicleRegistrationNumber vehicleReg, CancellationToken cancellationToken)
        => context.Set<ParkingEntity>().FirstOrDefaultAsync(e => e.VehicleReg == vehicleReg.Value && !e.TimeOut.HasValue, cancellationToken);
}
