namespace ParkingManagement.Domain.Repositories;

/// <summary>
/// The parking repository interface for data access operations.
/// </summary>
public interface IParkingRepository
{
    /// <summary>
    /// Finds a parking record by vehicle registration number.
    /// </summary>
    /// <param name="vehicleReg"></param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The <see cref="Parking"/> record if found, otherwise null"/>.</returns>
    Task<Parking?> Find(VehicleRegistrationNumber vehicleReg, CancellationToken cancellationToken);

    /// <summary>
    /// Saves a parking record.
    /// </summary>
    /// <param name="record">The <see cref="Parking"/> record.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A .</returns>
    Task Save(Parking record, CancellationToken cancellationToken);
}
