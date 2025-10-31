using ParkingManagement.Domain;

namespace ParkingManagement.Application.Configuration;

internal sealed class ChargingOptions
{
    public const string SectionName = "Charging";

    public Dictionary<VehicleType, decimal> Basic { get; set; } = [];

    public AdditionalCharge Additional { get; set; } = new(0, 0);

    public record AdditionalCharge(int Interval, decimal Charge);
}
