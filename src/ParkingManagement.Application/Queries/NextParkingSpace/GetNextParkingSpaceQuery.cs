using FluentResults;

namespace ParkingManagement.Application.Queries.NextParkingSpace;

/// <summary>
/// Represents a query to retrieve next available parking space.
/// </summary>
public sealed record GetNextParkingSpaceQuery : IApplicationQuery<Result<NextParkingSpaceResponse>>;
