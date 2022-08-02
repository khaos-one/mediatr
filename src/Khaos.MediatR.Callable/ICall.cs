using MediatR;

namespace Khaos.MediatR.Callable;

public interface ICall<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Send(TRequest request, CancellationToken cancellationToken = default);
}

public interface ICall<in TRequest> : ICall<TRequest, Unit> 
    where TRequest : IRequest<Unit>
{ }