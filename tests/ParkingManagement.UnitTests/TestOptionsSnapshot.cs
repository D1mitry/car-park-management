using Microsoft.Extensions.Options;

namespace ParkingManagement.UnitTests;

internal class TestOptionsSnapshot<T>(T value) : IOptionsSnapshot<T> where T : class, new()
{
    public T Value => value;

    public T Get(string? name) => value; // ignore name in tests
}
