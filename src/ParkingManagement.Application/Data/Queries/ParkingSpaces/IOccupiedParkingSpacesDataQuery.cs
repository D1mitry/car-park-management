namespace ParkingManagement.Application.Data.Queries.ParkingSpaces;

/// <summary>
/// The data query to fetch information about occupied parking spaces.
/// </summary>
public interface IOccupiedParkingSpacesDataQuery
{
    /// <summary>
    /// Retrieves the count of occupied parking spaces.
    /// </summary>
    /// <returns></returns>
    Task<int> Count();

    /// <summary>
    /// Gets the occupied parking spaces.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only collections of occupied parking spaces numbers.</returns>
    Task<IReadOnlyCollection<int>> Get();
}
