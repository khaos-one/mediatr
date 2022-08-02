using MediatR;

using Microsoft.Extensions.Logging;

namespace Khaos.MediatR.Rpc.Sandbox.Client;

public sealed class LoggingPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingPipelineBehaviour<TRequest, TResponse>> _logger;

    public LoggingPipelineBehaviour(ILogger<LoggingPipelineBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        _logger.LogInformation("Sending HTTP request through mediatr.");
        
        var response = await next();
       
        _logger.LogInformation("Done sending HTTP request through mediatr.");

        return response;
    }
}