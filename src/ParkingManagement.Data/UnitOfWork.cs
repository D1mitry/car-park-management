using ParkingManagement.Application.Infrastructure;

namespace ParkingManagement.Data;

internal sealed class UnitOfWork(IDataStore dataStore) : IUnitOfWork
{
    public Task Commit(CancellationToken cancellationToken = default)
        => dataStore.SaveChanges(cancellationToken);
}
