namespace Khaos.MediatR.Rpc.AspNetCore;

public record HttpDelegate(Delegate Delegate, Type? VisibleReturnType);