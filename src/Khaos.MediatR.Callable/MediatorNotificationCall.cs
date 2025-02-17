using MediatR;

namespace Khaos.MediatR.Callable;

public sealed class MediatorNotificationCall<TNotification>(IMediator mediator) 
    : INotificationCall<TNotification>
    where TNotification : INotification
{
    public Task Publish(TNotification notification, CancellationToken cancellationToken = default) =>
        mediator.Publish(notification, cancellationToken);
}