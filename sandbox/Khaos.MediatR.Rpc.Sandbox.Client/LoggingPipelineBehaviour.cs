using MediatR;

using Microsoft.Extensions.Logging;

namespace Khaos.MediatR.Rpc.Sandbox.Client;

public sealed class LoggingPipelineBehaviour<TRequest, TResponse>(
    ILogger<LoggingPipelineBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogInformation("Sending HTTP request through mediatr.");
        
        var response = await next();
       
        logger.LogInformation("Done sending HTTP request through mediatr.");

        return response;
    }
}