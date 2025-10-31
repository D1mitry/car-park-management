using Microsoft.Extensions.Options;
using System.Collections.Immutable;

namespace ParkingManagement.Application.Configuration;

internal sealed class ParkingOptionsSetup : IConfigureOptions<ParkingOptions>
{
    public void Configure(ParkingOptions options)
    {
        if ((options.Spaces is null || options.Spaces.Count == 0) && options.TotalSpaces > 0)
            options.Spaces = Enumerable.Range(1, options.TotalSpaces).ToImmutableList();
    }
}
