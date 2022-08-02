using System.Net;

namespace Khaos.MediatR.Rpc.Client;

public class RpcClientException : Exception
{
    public HttpResponseMessage ResponseMessage { get; }

    public RpcClientException(string message, Exception? inner, HttpResponseMessage responseMessage)
        : base(message, inner)
    {
        ResponseMessage = responseMessage;
    }
    
    public RpcClientException(string message, HttpResponseMessage responseMessage)
        : this(message, null, responseMessage)
    { }
}