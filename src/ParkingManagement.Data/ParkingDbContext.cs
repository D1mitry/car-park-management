using Microsoft.EntityFrameworkCore;
using ParkingManagement.Application.Infrastructure;
using ParkingManagement.Data.Conversions;
using ParkingManagement.Data.Entities;

namespace ParkingManagement.Data;

internal sealed class ParkingDbContext(DbContextOptions options) : DbContext(options), IDataStore
{
    public Task SaveChanges(CancellationToken cancellationToken = default)
        => SaveChangesAsync(cancellationToken);

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverver>();
        configurationBuilder.Properties<DateTime>().HaveConversion<UtcNullableDateTimeConverver>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ParkingEntity>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ParkingSpaceNumber)
                .IsRequired();

            entity.Property(e => e.VehicleReg)
                .IsRequired();

            entity.Property(e => e.VehicleType)
                .IsRequired();

            entity.Property(e => e.TimeIn)
                  .IsRequired();

            entity.Property(e => e.TimeOut);

            entity.HasIndex(e => new { e.ParkingSpaceNumber, e.TimeOut })
                .IsUnique();

            entity.HasIndex(e => new { e.VehicleReg, e.TimeOut })
                .IsUnique();
        });
    }
}
