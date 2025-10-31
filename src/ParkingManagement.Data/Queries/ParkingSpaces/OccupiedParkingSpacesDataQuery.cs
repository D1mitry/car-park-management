using Microsoft.EntityFrameworkCore;
using ParkingManagement.Application.Data.Queries.ParkingSpaces;
using ParkingManagement.Data.Entities;

namespace ParkingManagement.Data.Queries.ParkingSpaces;

internal sealed class OccupiedParkingSpacesDataQuery(ParkingDbContext context) : IOccupiedParkingSpacesDataQuery
{
    private IQueryable<int> Query
        => context.Set<ParkingEntity>()
          .AsNoTracking()
          .Where(p => !p.TimeOut.HasValue)
          .Select(p => p.ParkingSpaceNumber);

    public Task<int> Count()
        => Query.CountAsync();

    public async Task<IReadOnlyCollection<int>> Get()
        => await Query.ToListAsync();
}
