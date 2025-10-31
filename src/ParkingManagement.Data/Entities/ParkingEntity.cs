using ParkingManagement.Domain;

namespace ParkingManagement.Data.Entities;

internal sealed class ParkingEntity
{
    public required Guid Id { get; init; }

    public required int ParkingSpaceNumber { get; init; }

    public required string VehicleReg { get; init; }

    public required VehicleType VehicleType { get; init; }

    public required DateTime TimeIn { get; init; }

    public DateTime? TimeOut { get; set; }
}
