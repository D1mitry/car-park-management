namespace ParkingManagement.Application.Infrastructure;

/// <summary>
/// The Unit of Work interface for managing transactions.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Commits the current transaction asynchronously.
    /// </summary>
    /// <remarks>This method finalizes the transaction, making all changes permanent. If the operation
    /// is canceled via the <paramref name="cancellationToken"/>, the transaction will not be committed.</remarks>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous commit operation.</returns>
    Task Commit(CancellationToken cancellationToken = default);
}
