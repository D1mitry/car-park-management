using System.Text.Json.Serialization;

namespace ParkingManagement.Domain;

/// <summary>
/// An enumeration of vehicle types.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VehicleType
{
    /// <summary>
    /// A small car.
    /// </summary>
    SmallCar,

    /// <summary>
    /// A medium car.
    /// </summary>
    MediumCar,

    /// <summary>
    /// A large car.
    /// </summary>
    LargeCar,
}
