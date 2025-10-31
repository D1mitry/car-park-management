using DomainBase;
using ParkingManagement.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace ParkingManagement.Domain;

/// <summary>
/// The vehicle registration number value object.
/// </summary>
public sealed partial class VehicleRegistrationNumber : ValueObject<VehicleRegistrationNumber, string>
{
    private VehicleRegistrationNumber(string value) : base(value)
    {
    }

    /// <summary>
    /// Creates a new <see cref="VehicleRegistrationNumber"/> instance.
    /// </summary>
    /// <param name="registration">The vehicle registration number.</param>
    /// <returns>A newly created <see cref="VehicleRegistrationNumber"/></returns>
    public static VehicleRegistrationNumber Create(string registration)
    {
        if (string.IsNullOrWhiteSpace(registration))
            throw registration.BecauseEmptyRegistrationNumber();

        registration = registration.Trim().ToUpperInvariant();

        if (!RegistrationNumberPattern().IsMatch(registration))
            throw registration.BecauseInvalidRegistrationNumber();

        return new VehicleRegistrationNumber(registration);
    }

    [GeneratedRegex(@"^[A-Za-z0-9]{1,10}$")]
    private static partial Regex RegistrationNumberPattern();
}
