using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ParkingManagement.Data.Conversions;

internal sealed class UtcDateTimeConverver() : ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

internal sealed class UtcNullableDateTimeConverver() : ValueConverter<DateTime?, DateTime?>(v => v, v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);
