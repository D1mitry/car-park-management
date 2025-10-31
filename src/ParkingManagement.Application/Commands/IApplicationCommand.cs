using MediatR;

namespace ParkingManagement.Application.Commands;

/// <summary>
/// Represents a command that can be executed within an application.
/// </summary>
internal interface IApplicationCommand;

/// <summary>
/// Represents a command within the application that, when executed, returns a response of type
/// <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by executing the command.</typeparam>
internal interface IApplicationCommand<out TResponse> : IApplicationCommand, IRequest<TResponse>;
