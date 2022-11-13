namespace Khaos.MediatR.Rpc.AspNetCore;

public interface IHttpDelegateFactory
{
    public HttpDelegate Construct(Type requestType);
}