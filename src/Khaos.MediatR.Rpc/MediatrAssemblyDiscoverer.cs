using System.Collections.Immutable;
using MediatR;

namespace Khaos.MediatR.Rpc;

public class MediatrAssemblyDiscoverer
{
    private readonly Type[] _markerTypes;

    public MediatrAssemblyDiscoverer(Type[] markerTypes)
    {
        _markerTypes = markerTypes;
    }

    public IEnumerable<(Type MarkerType, Type MediatrType)> EnumerateMediatrTypes()
    {
        var assemblies = _markerTypes
            .Select(type => (type, type.Assembly))
            .ToImmutableHashSet();
        
        foreach (var (markerType, assembly) in assemblies)
        {
            var matchingTypes = assembly.GetExportedTypes()
                .Where(
                    type =>
                        !type.IsAbstract
                        && !type.IsInterface
                        && type.IsAssignableTo(typeof(IBaseRequest)));

            foreach (var type in matchingTypes)
            {
                yield return (markerType, type);
            }
        }
    }
}