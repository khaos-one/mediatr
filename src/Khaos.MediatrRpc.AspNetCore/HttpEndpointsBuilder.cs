using System.Linq.Expressions;
using System.Reflection;
using MediatR;

namespace Khaos.MediatrRpc.AspNetCore;

public class HttpEndpointsBuilder
{
    private readonly MediatrAssemblyDiscoverer _discoverer;

    public HttpEndpointsBuilder(MediatrAssemblyDiscoverer discoverer)
    {
        _discoverer = discoverer;
    }

    public IEnumerable<HttpEndpoint> EnumerateEndpoints()
    {
        foreach (var mediatrType in _discoverer.EnumerateMediatrTypes())
        {
            yield return new(
                TypeRouteNameFactory.Get(mediatrType),
                ConstructHandler(mediatrType));
        }
    }

    internal static Delegate ConstructHandler(Type requestType)
    {
        Delegate concreteHandler;

        var genericHandlerMethodBuilder = GetGenericHandlerMethodBuilder();
        var genericHandlerMethodBuilderWithReturnType = GetGenericHandlerMethodBuilderWithReturnType();

        var typedRequestInterface = requestType.GetInterface("IRequest`1");

        if (typedRequestInterface is not null)
        {
            var returnType = typedRequestInterface.GetGenericArguments().FirstOrDefault();

            concreteHandler =
                (Delegate) genericHandlerMethodBuilderWithReturnType
                    .MakeGenericMethod(requestType, returnType!)
                    .Invoke(null, Array.Empty<object>())!;
        }
        else
        {
            concreteHandler =
                (Delegate) genericHandlerMethodBuilder
                    .MakeGenericMethod(requestType)
                    .Invoke(null, Array.Empty<object>())!;
        }

        if (concreteHandler is null)
        {
            throw new InvalidOperationException("Cannot create concrete handler for specified request type.");
        }

        return concreteHandler!;
    }

    private static MethodInfo GetGenericHandlerMethodBuilder()
    {
        Expression<Func<Func<IRequest, IMediator, CancellationToken, Task>>> expr = () => GetGenericHandler<IRequest>();
        var methodCallExpr = (MethodCallExpression) expr.Body;
        return methodCallExpr.Method.GetGenericMethodDefinition();
    }

    private static Func<T, IMediator, CancellationToken, Task> GetGenericHandler<T>()
        where T : IRequest =>
        async (T request, IMediator mediatr, CancellationToken cancellationToken) =>
            await mediatr.Send(request, cancellationToken);

    private static MethodInfo GetGenericHandlerMethodBuilderWithReturnType()
    {
        Expression<Func<Func<DummyRequest, IMediator, CancellationToken, Task>>> expr = () =>
            GetGenericHandlerWithReturnType<DummyRequest, object>();
        var methodCallExpr = (MethodCallExpression) expr.Body;
        return methodCallExpr.Method.GetGenericMethodDefinition();
    }

    private static Func<TIn, IMediator, CancellationToken, Task<TOut>> GetGenericHandlerWithReturnType<TIn, TOut>()
        where TIn : IRequest<TOut> =>
        async (TIn request, IMediator mediatr, CancellationToken cancellationToken) =>
            await mediatr.Send(request, cancellationToken);

    private sealed class DummyRequest : IRequest<object>
    { }
}