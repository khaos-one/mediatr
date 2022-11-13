using System.Linq.Expressions;
using System.Reflection;

using MediatR;

namespace Khaos.MediatR.Rpc.AspNetCore;

public class PlainHttpDelegateFactory : IHttpDelegateFactory
{
    public HttpDelegate Construct(Type requestType)
    {
        Delegate concreteHandler;

        var returnType = RequestReturnTypeExtractor.TryGetReturnType(requestType);
        
        if (returnType is not null)
        {
            concreteHandler =
                (Delegate) GetGenericHandlerMethodBuilderWithReturnType()
                    .MakeGenericMethod(requestType, returnType)
                    .Invoke(null, Array.Empty<object>())!;
        }
        else
        {
            concreteHandler =
                (Delegate) GetGenericHandlerMethodBuilder()
                    .MakeGenericMethod(requestType)
                    .Invoke(null, Array.Empty<object>())!;
        }

        if (concreteHandler is null)
        {
            throw new InvalidOperationException("Cannot create concrete handler for specified request type.");
        }

        return new HttpDelegate(concreteHandler, returnType);
    }

    private static MethodInfo GetGenericMethodBuilder(MethodCallExpression expression) =>
        expression.Method.GetGenericMethodDefinition();

    private static MethodInfo GetGenericHandlerMethodBuilder()
    {
        Expression<Func<Func<IRequest, IMediator, CancellationToken, Task>>> expr = 
            () => GetGenericHandler<IRequest>();

        return GetGenericMethodBuilder((MethodCallExpression) expr.Body);
    }

    private static Func<T, IMediator, CancellationToken, Task> GetGenericHandler<T>()
        where T : IRequest =>
        async (T request, IMediator mediatr, CancellationToken cancellationToken) =>
            await mediatr.Send(request, cancellationToken);

    private static MethodInfo GetGenericHandlerMethodBuilderWithReturnType()
    {
        Expression<Func<Func<DummyRequest, IMediator, CancellationToken, Task>>> expr = 
            () => GetGenericHandlerWithReturnType<DummyRequest, object>();

        return GetGenericMethodBuilder((MethodCallExpression) expr.Body);
    }

    private static Func<TIn, IMediator, CancellationToken, Task<TOut>> GetGenericHandlerWithReturnType<TIn, TOut>()
        where TIn : IRequest<TOut> =>
        async (TIn request, IMediator mediatr, CancellationToken cancellationToken) =>
            await mediatr.Send(request, cancellationToken);

    private sealed class DummyRequest : IRequest<object>
    { }
}