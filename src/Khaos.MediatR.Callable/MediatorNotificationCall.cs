using MediatR;

namespace Khaos.MediatR.Callable;

public sealed class MediatorNotificationCall<TNotification> 
    : INotificationCall<TNotification> where TNotification : INotification
{
    private readonly IMediator _mediator;

    public MediatorNotificationCall(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task Publish(TNotification notification, CancellationToken cancellationToken = default) =>
        _mediator.Publish(notification, cancellationToken);
}