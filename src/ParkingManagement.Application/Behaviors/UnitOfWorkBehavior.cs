using MediatR;
using ParkingManagement.Application.Commands;
using ParkingManagement.Application.Infrastructure;

namespace ParkingManagement.Application.Behaviors;

internal sealed class UnitOfWorkBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        TResponse response = await next(cancellationToken);

        if (request is IApplicationCommand<TResponse>)
            await unitOfWork.Commit(cancellationToken);

        return response;
    }
}
