using System.Linq.Expressions;
using System.Reflection;

using MediatR;

namespace Khaos.MediatR.Rpc.AspNetCore;

public class NewtonsoftJsonHttpDelegateFactory : IHttpDelegateFactory
{
    public HttpDelegate Construct(Type requestType)
    {
        Delegate concreteHandler;

        var returnType = RequestReturnTypeExtractor.TryGetReturnType(requestType);
        var wrapperType = typeof(NewtonsoftJsonTypeWrapper<>).MakeGenericType(requestType);
        
        if (returnType is not null)
        {
            concreteHandler =
                (Delegate) GetGenericHandlerMethodBuilderWithReturnType()
                    .MakeGenericMethod(wrapperType, returnType)
                    .Invoke(null, Array.Empty<object>())!;
        }
        else
        {
            concreteHandler =
                (Delegate) GetGenericHandlerMethodBuilder()
                    .MakeGenericMethod(wrapperType)
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
        Expression<Func<Func<NewtonsoftJsonTypeWrapper<IRequest>, IMediator, CancellationToken, Task>>> expr = 
            () => GetGenericHandler<NewtonsoftJsonTypeWrapper<IRequest>>();

        return GetGenericMethodBuilder((MethodCallExpression) expr.Body);
    }

    private static Func<T, IMediator, CancellationToken, Task> GetGenericHandler<T>()
        where T : NewtonsoftJsonTypeWrapper<IRequest> =>
        async (T request, IMediator mediatr, CancellationToken cancellationToken) =>
            await mediatr.Send(request.Value!, cancellationToken);

    private static MethodInfo GetGenericHandlerMethodBuilderWithReturnType()
    {
        Expression<Func<Func<DummyRequest, IMediator, CancellationToken, Task>>> expr = 
            () => GetGenericHandlerWithReturnType<DummyRequest, object>();

        return GetGenericMethodBuilder((MethodCallExpression) expr.Body);
    }

    private static Func<TIn, IMediator, CancellationToken, Task<TOut>> GetGenericHandlerWithReturnType<TIn, TOut>()
        where TIn : NewtonsoftJsonTypeWrapper<IRequest<TOut>> =>
        async (TIn request, IMediator mediatr, CancellationToken cancellationToken) =>
            await mediatr.Send(request.Value!, cancellationToken);

    private sealed class DummyRequest : NewtonsoftJsonTypeWrapper<IRequest<object>>
    {
        public DummyRequest(IRequest<object>? inner) 
            : base(inner)
        { }
    }
}