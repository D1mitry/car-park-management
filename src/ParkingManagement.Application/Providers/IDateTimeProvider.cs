namespace ParkingManagement.Application.Providers;

/// <summary>
/// The date and time service interface.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Returns the current date and time in UTC.
    /// </summary>
    DateTime UtcNow { get; }
}
