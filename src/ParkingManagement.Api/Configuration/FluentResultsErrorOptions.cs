using FluentResults;
using System.Collections.Concurrent;

namespace ParkingManagement.Api.Configuration;

internal sealed class FluentResultsErrorOptions
{
    private readonly ConcurrentDictionary<Type, (int, string)> _errorStatusMappings = new();

    public void MapError<TError>(int statusCode, string type) where TError : Error
    {
        _errorStatusMappings[typeof(TError)] = (statusCode, type);
    }

    public (int StatusCode, string Type)? GetErrorDetails(IError error)
    {
        var type = error.GetType();
        if (_errorStatusMappings.TryGetValue(type, out var code))
            return code;

        // maybe walk base types
        var baseType = type.BaseType;
        while (baseType != null && baseType != typeof(Error))
        {
            if (_errorStatusMappings.TryGetValue(baseType, out code))
                return code;

            baseType = baseType.BaseType;
        }

        return null;
    }
}
