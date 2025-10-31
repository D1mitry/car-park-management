namespace ParkingManagement.Application.Queries.ParkingSpaces;

/// <summary>
/// Represents a query to retrieve information about available parking spaces.
/// </summary>
public sealed record GetParkingSpacesQuery : IApplicationQuery<ParkingSpacesQueryResponse>;
