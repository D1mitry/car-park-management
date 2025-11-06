using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using ParkingManagement.Application.Configuration;
using ParkingManagement.Application.Data.Queries.ParkingSpaces;
using ParkingManagement.Application.Infrastructure.Errors;
using System.Collections.Immutable;

namespace ParkingManagement.Application.Queries.NextParkingSpace;

internal sealed class GetNextParkingSpaceQueryHandler(
    IOccupiedParkingSpacesDataQuery query,
    IOptions<ParkingOptions> parkingOptions)
    : IRequestHandler<GetNextParkingSpaceQuery, Result<NextParkingSpaceResponse>>
{
    public async Task<Result<NextParkingSpaceResponse>> Handle(GetNextParkingSpaceQuery request, CancellationToken cancellationToken)
    {
        var availableParkingSpaces = parkingOptions.Value.Spaces.Except(await query.Get()).ToImmutableList();

        var nextParkingSpace = availableParkingSpaces.FirstOrDefault();

        return nextParkingSpace == 0
            ? ConflictError.BecauseNoAvailableParkingSpaces()
            : new NextParkingSpaceResponse(nextParkingSpace, availableParkingSpaces.Count);
    }
}
