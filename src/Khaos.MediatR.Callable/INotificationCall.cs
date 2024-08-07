using MediatR;

namespace Khaos.MediatR.Callable;

public interface INotificationCall<in TNotification>
    where TNotification : INotification
{
    Task Publish(TNotification notification, CancellationToken cancellationToken = default);
}