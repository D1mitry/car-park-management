namespace ParkingManagement.Application.Queries.ParkingSpaces;

/// <summary>
/// Represents the response to a query for parking spaces availability.
/// </summary>
/// <param name="AvailableSpaces">The number of available parking spaces.</param>
/// <param name="OccupiedSpaces">The number of occupied parking spaces.</param>
public sealed record ParkingSpacesQueryResponse(int AvailableSpaces, int OccupiedSpaces);
