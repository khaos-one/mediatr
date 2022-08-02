using Microsoft.Extensions.DependencyInjection;

namespace Khaos.MediatR.Callable;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediatRCallables(this IServiceCollection services)
    {
        return services
            .AddTransient(typeof(ICall<,>), typeof(MediatorCall<,>))
            .AddTransient(typeof(INotificationCall<>), typeof(MediatorNotificationCall<>));
    }
}