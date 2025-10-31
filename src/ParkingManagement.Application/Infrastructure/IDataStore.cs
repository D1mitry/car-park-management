namespace ParkingManagement.Application.Infrastructure;

/// <summary>
/// The data store interface.
/// </summary>
public interface IDataStore
{
    /// <summary>
    /// Saves the data asynchronously.
    /// </summary>
    /// <remarks>This method saves the all changes. If the operation
    /// is canceled via the <paramref name="cancellationToken"/>, the data will not be saved.</remarks>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous commit operation.</returns>
    Task SaveChanges(CancellationToken cancellationToken = default);
}
