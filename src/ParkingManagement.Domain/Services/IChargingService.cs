using DomainBase;

namespace ParkingManagement.Domain.Services;

/// <summary>
/// The charging domain service.
/// </summary>
public interface IChargingService : IDomainService
{
    /// <summary>
    /// Calculates the parking charge for the given record.
    /// </summary>
    /// <param name="record">The parking record</param>
    /// <returns></returns>
    decimal CalculateCharge(Parking record);
}
