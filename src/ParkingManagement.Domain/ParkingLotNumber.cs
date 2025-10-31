using DomainBase;

namespace ParkingManagement.Domain;

/// <summary>
/// The parking space number value object.
/// </summary>
public sealed partial class ParkingSpaceNumber : ValueObject<ParkingSpaceNumber, int>
{
    private ParkingSpaceNumber(int value) : base(value)
    {
    }

    /// <summary>
    /// Creates a new <see cref="ParkingSpaceNumber"/> instance.
    /// </summary>
    /// <param name="number">The number of the parking space.</param>
    /// <returns>A newly created <see cref="ParkingSpaceNumber"/>.</returns>
    public static ParkingSpaceNumber Create(int number)
        => new(number);
}
