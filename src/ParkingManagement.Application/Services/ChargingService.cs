using Microsoft.Extensions.Options;
using ParkingManagement.Application.Configuration;
using ParkingManagement.Domain;
using ParkingManagement.Domain.Exceptions;
using ParkingManagement.Domain.Services;

namespace ParkingManagement.Application.Services;

internal sealed class ChargingService(
    IOptions<ChargingOptions> chargingOptions)
    : IChargingService
{
    public decimal CalculateCharge(Parking record)
    {
        if (!record.TimeOut.HasValue)
            throw record.BecauseTimeOutEmpty();

        if (!chargingOptions.Value.Basic.TryGetValue(record.VehicleType, out decimal basicCharge))
            throw record.BecauseBasicChargeMissing();

        int duration = (int)Math.Round((record.TimeOut.Value - record.TimeIn).TotalMinutes, 0, MidpointRounding.ToZero);
        int additionalIntervals = duration / chargingOptions.Value.Additional.Interval;

        decimal totalCharge = duration * basicCharge;
        if (additionalIntervals > 0)
            totalCharge += additionalIntervals * chargingOptions.Value.Additional.Charge;

        return totalCharge;
    }
}
