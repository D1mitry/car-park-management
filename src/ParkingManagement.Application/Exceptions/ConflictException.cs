namespace ParkingManagement.Application.Exceptions;

/// <summary>
/// The application conflict exception.
/// </summary>
public sealed class ConflictException : Exception
{
    private ConflictException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Throws when the vehicle is already parked.
    /// </summary>
    /// <param name="vehicleReg">The registration number of the vehicle.</param>
    public static ConflictException BecauseVehicleAlreadyParked(string vehicleReg)
        => new($"Vehicle with registration number '{vehicleReg}' is already parked.");

    /// <summary>
    /// Throws when there are no available parking spaces.
    /// </summary>
    public static ConflictException BecauseNoAvailableParkingSpaces()
        => new("No available parking spaces.");
}
