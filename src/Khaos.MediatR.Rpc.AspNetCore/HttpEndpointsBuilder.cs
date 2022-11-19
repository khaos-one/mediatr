using System.Net.Http.Headers;
using System.Text;

using Khaos.MediatR.Rpc.AspNetCore.Metadata;
using Khaos.MediatR.Rpc.Codecs;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Khaos.MediatR.Rpc.AspNetCore;

internal sealed class HttpEndpointsBuilder
{
    private readonly MediatrAssemblyDiscoverer _discoverer;
    private readonly IStreamCodecFactory _streamCodecFactory;
    private readonly CodecMetadataEmitterRegistry _codecMetadataEmitterRegistry;

    public HttpEndpointsBuilder(
        MediatrAssemblyDiscoverer discoverer,
        IStreamCodecFactory streamCodecFactory,
        CodecMetadataEmitterRegistry codecMetadataEmitterRegistry)
    {
        _discoverer = discoverer;
        _streamCodecFactory = streamCodecFactory;
        _codecMetadataEmitterRegistry = codecMetadataEmitterRegistry;
    }

    public void Build(IEndpointRouteBuilder routeBuilder)
    {
        foreach (var (markerType, mediatrType) in _discoverer.EnumerateMediatrTypes())
        {
            var streamCodec = _streamCodecFactory.GetOrDefault(markerType);
            
            RequestDelegate requestHandler = async httpContext =>
            {
                if (!(MediaTypeHeaderValue.TryParse(httpContext.Request.ContentType, out var mt) 
                        && streamCodec.SupportedContentTypes.Contains(
                            mt.MediaType!,
                            StringComparer.InvariantCultureIgnoreCase)))
                {
                    throw new BadHttpRequestException(
                        "Request content type is not acceptable for this request.",
                        StatusCodes.Status415UnsupportedMediaType);
                }

                var request = await streamCodec.Decode(
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

                using var outputStream = new MemoryStream();
                await streamCodec.Encode(result, outputStream, httpContext.RequestAborted);

                httpContext.Response.ContentType = streamCodec.OutputContentType;
                httpContext.Response.ContentLength = outputStream.Length;

                await httpContext.Response.StartAsync();
                await httpContext.Response.BodyWriter.WriteAsync(outputStream.ToArray(), httpContext.RequestAborted);
                await httpContext.Response.BodyWriter.FlushAsync();
            };

            var conventionBuilder = routeBuilder
                .MapPost(TypeRoutePathFactory.Get(mediatrType), requestHandler)
                .WithMetadata(requestHandler.Method);
            var codecMetadataEmitter = _codecMetadataEmitterRegistry.Get(streamCodec.GetType());

            codecMetadataEmitter.EmitForType(conventionBuilder, mediatrType);
        }
    }
}