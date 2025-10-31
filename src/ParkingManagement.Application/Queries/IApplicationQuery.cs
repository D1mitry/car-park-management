using MediatR;

namespace ParkingManagement.Application.Queries;

/// <summary>
/// Represents a query that can be executed within an application.
/// </summary>
internal interface IApplicationQuery;

/// <summary>
/// Represents a query within the application that, when executed, returns a response of type
/// <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by executing the query.</typeparam>
internal interface IApplicationQuery<out TResponse> : IApplicationQuery, IRequest<TResponse>;
