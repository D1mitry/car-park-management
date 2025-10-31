namespace ParkingManagement.Application.Providers;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow
        => DateTime.UtcNow;
}
