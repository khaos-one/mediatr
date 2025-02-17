using MediatR;

namespace Khaos.MediatR.Callable;

public class MediatorCall<TRequest, TResponse>(IMediator mediator) 
    : ICall<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task<TResponse> Send(TRequest request, CancellationToken cancellationToken = default) =>
        mediator.Send(request, cancellationToken);
}

public sealed class MediatorCall<TRequest>(IMediator mediator)
    : ICall<TRequest> 
    where TRequest : IRequest
{
    public Task Send(TRequest request, CancellationToken cancellationToken = default) =>
        mediator.Send(request, cancellationToken);
}