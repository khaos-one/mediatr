using MediatR;

namespace Khaos.MediatR.Callable;

public class MediatorCall<TRequest, TResponse> 
    : ICall<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IMediator _mediator;

    public MediatorCall(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<TResponse> Send(TRequest request, CancellationToken cancellationToken = default) =>
        _mediator.Send(request, cancellationToken);
}

public sealed class MediatorCall<TRequest> 
    : MediatorCall<TRequest, Unit>, ICall<TRequest> where TRequest : IRequest<Unit>
{
    public MediatorCall(IMediator mediator) 
        : base(mediator)
    { }
}