using System.Collections.Concurrent;

namespace Khaos.MediatR.Rpc.Client;

internal class ClientTypesCache
{
    private readonly ConcurrentDictionary<Type, Entry> _cache = new();

    public Entry GetOrCreateForType(Type type) => 
        _cache.GetOrAdd(type, Entry.CreateForType);
    
    internal record Entry(Type? ResponseType, string RoutePath)
    {
        public static Entry CreateForType(Type requestType)
        {
            return new(
                RequestReturnTypeExtractor.TryGetReturnType(requestType), 
                TypeRoutePathFactory.Get(requestType));
        }
    }
}