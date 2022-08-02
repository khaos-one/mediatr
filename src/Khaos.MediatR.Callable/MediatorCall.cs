using MediatR;

namespace Khaos.MediatR.Callable;

public sealed class MediatorCall<TRequest, TResponse> 
    : ICall<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IMediator _mediator;

    public MediatorCall(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<TResponse> Send(TRequest request, CancellationToken cancellationToken) =>
        _mediator.Send(request, cancellationToken);
}