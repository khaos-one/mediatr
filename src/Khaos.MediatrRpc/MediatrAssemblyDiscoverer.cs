using System.Collections.Immutable;
using MediatR;

namespace Khaos.MediatrRpc;

public class MediatrAssemblyDiscoverer
{
    private readonly Type[] _markerTypes;

    public MediatrAssemblyDiscoverer(Type[] markerTypes)
    {
        _markerTypes = markerTypes;
    }

    public IEnumerable<Type> EnumerateMediatrTypes()
    {
        var assemblies = _markerTypes
            .Select(type => type.Assembly)
            .ToImmutableHashSet();
        
        foreach (var assembly in assemblies)
        {
            var matchingTypes = assembly.GetExportedTypes()
                .Where(
                    type =>
                        !type.IsAbstract
                        && !type.IsInterface
                        && type.IsAssignableTo(typeof(IBaseRequest)));

            foreach (var type in matchingTypes)
            {
                yield return type;
            }
        }
    }
}