using System.Collections.Concurrent;

namespace ParkingManagement.Api.Configuration;

internal sealed class ExceptionErrorOptions
{
    private readonly ConcurrentDictionary<Type, (int, string)> _exceptionStatusMappings = new();

    public void MapException<TException>(int statusCode, string type) where TException : Exception
    {
        _exceptionStatusMappings[typeof(TException)] = (statusCode, type);
    }

    public (int StatusCode, string Type)? GetErrorDetails(Exception exception)
    {
        var type = exception.GetType();
        if (_exceptionStatusMappings.TryGetValue(type, out var code))
            return code;

        // maybe walk base types
        var baseType = type.BaseType;
        while (baseType != null && baseType != typeof(Exception))
        {
            if (_exceptionStatusMappings.TryGetValue(baseType, out code))
                return code;

            baseType = baseType.BaseType;
        }

        return null;
    }
}
