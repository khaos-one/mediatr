using System.Net.Http.Headers;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Khaos.MediatR.Rpc.AspNetCore;

public class HttpEndpointsBuilder
{
    private readonly MediatrAssemblyDiscoverer _discoverer;
    private readonly IStreamCodecAndMetadataEmitter _streamCodec;

    public HttpEndpointsBuilder(MediatrAssemblyDiscoverer discoverer, IStreamCodecAndMetadataEmitter streamCodec)
    {
        _discoverer = discoverer;
        _streamCodec = streamCodec;
    }

    public void Build(IEndpointRouteBuilder routeBuilder)
    {
        foreach (var mediatrType in _discoverer.EnumerateMediatrTypes())
        {
            RequestDelegate requestHandler = async httpContext =>
            {
                if (!(MediaTypeHeaderValue.TryParse(httpContext.Request.ContentType, out var mt) 
                      && _streamCodec.SupportedContentTypes.Contains(
                            mt.MediaType!,
                            StringComparer.InvariantCultureIgnoreCase)))
                {
                    throw new BadHttpRequestException(
                        "Request context type is not acceptable for this request.",
                        StatusCodes.Status415UnsupportedMediaType);
                }

                var request = await _streamCodec.Decode(
                    mediatrType,
                    httpContext.Request.Body,
                    httpContext.RequestAborted);

                if (request is null)
                {
                    throw new BadHttpRequestException(
                        "Request was null.",
                        StatusCodes.Status400BadRequest);
                }

                var mediatr = httpContext.RequestServices.GetRequiredService<IMediator>();
                var result = await mediatr.Send(request, httpContext.RequestAborted);

                await using var outputStream = httpContext.Response.BodyWriter.AsStream();
                await _streamCodec.Encode(
                    result,
                    outputStream,
                    httpContext.RequestAborted);
            };

            routeBuilder
                .MapPost(TypeRoutePathFactory.Get(mediatrType), requestHandler)
                .WithMetadata(requestHandler.Method)
                .WithMetadata(_streamCodec.CreateAcceptsMetadataForType(mediatrType))
                .WithMetadata(
                    _streamCodec.CreateProducesMetadataForType(
                        RequestReturnTypeExtractor.TryGetReturnType(mediatrType)!));
        }
    }
}