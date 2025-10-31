using MediatR;
using Microsoft.AspNetCore.Mvc;
using ParkingManagement.Application.Commands.LeaveParkingSpace;
using ParkingManagement.Application.Commands.ParkVehicle;
using ParkingManagement.Application.Queries.ParkingSpaces;
using ParkingManagement.Domain;
using System.ComponentModel.DataAnnotations;

namespace ParkingManagement.Api.Controllers;

/// <summary>
/// The Parking Controller
/// </summary>
/// <param name="mediator">The mediator.</param>
[ApiController]
[Route("[controller]")]
public sealed class ParkingController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Gets available and occupied number of spaces.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="ParkingSpacesQueryResponse"/> containing the counts of available and occupied parking spaces.</returns>
    [HttpGet]
    public Task<ParkingSpacesQueryResponse> Get(CancellationToken cancellationToken)
        => mediator.Send(new GetParkingSpacesQuery(), cancellationToken);

    /// <summary>
    /// Parks a given vehicle in the first available space and returns the vehicle and its space number
    /// </summary>
    /// <param name="request">The parking request.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The parked vehicle details.</returns>
    [HttpPost]
    public Task<ParkVehicleResponse> Park([FromBody] ParkingRequest request, CancellationToken cancellationToken)
        => mediator.Send(new ParkVehicleCommand(request.VehicleReg, request.VehicleType), cancellationToken);

    /// <summary>
    /// Should free up this vehicles space and return its final charge from its parking time until now.
    /// </summary>
    /// <param name="request">The exit parking request.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The vehicle parking details.</returns>
    [HttpPost("[action]")]
    public Task<LeaveParkingSpaceResponse?> Exit([FromBody] ExitParkingRequest request, CancellationToken cancellationToken)
        => mediator.Send(new LeaveParkingSpaceCommand(request.VehicleReg), cancellationToken);
}

/// <summary>
/// A parking request.
/// </summary>
/// <param name="VehicleReg">The registration number of the vehicle.</param>
/// <param name="VehicleType">The type of the vehicle.</param>
public record ParkingRequest([Required] string VehicleReg, [EnumDataType(typeof(VehicleType))] VehicleType VehicleType);

/// <summary>
/// An exit parking request.
/// </summary>
/// <param name="VehicleReg">The registration number of the vehicle.</param>
public record ExitParkingRequest([Required] string VehicleReg);
