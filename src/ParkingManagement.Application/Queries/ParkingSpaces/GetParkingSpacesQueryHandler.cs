using MediatR;
using Microsoft.Extensions.Options;
using ParkingManagement.Application.Configuration;
using ParkingManagement.Application.Data.Queries.ParkingSpaces;

namespace ParkingManagement.Application.Queries.ParkingSpaces;

internal sealed class GetParkingSpacesQueryHandler(
    IOccupiedParkingSpacesDataQuery query,
    IOptions<ParkingOptions> parkingOptions)
    : IRequestHandler<GetParkingSpacesQuery, ParkingSpacesQueryResponse>
{
    public async Task<ParkingSpacesQueryResponse> Handle(GetParkingSpacesQuery request, CancellationToken cancellationToken)
    {
        var occupied = await query.Count();

        return new ParkingSpacesQueryResponse(parkingOptions.Value.TotalSpaces - occupied, occupied);
    }
}
