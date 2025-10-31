namespace ParkingManagement.Application.Configuration;

internal sealed class ParkingOptions
{
    public const string SectionName = "Parking";

    public ushort TotalSpaces { get; set; }

    public IReadOnlyCollection<int> Spaces { get; set; } = [];
}
