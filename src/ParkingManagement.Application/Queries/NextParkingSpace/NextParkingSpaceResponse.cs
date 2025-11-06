namespace ParkingManagement.Application.Queries.NextParkingSpace;

/// <summary>
/// Represents the response to a query for the next available parking space.
/// </summary>
/// <param name="NextSpace">The number of the next parking space.</param>
/// <param name="AvailableSpaces">The count of available spaces.</param>
public sealed record NextParkingSpaceResponse(int NextSpace, int AvailableSpaces);
