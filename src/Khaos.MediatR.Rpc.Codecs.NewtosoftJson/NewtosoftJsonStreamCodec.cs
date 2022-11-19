using System.Text;

using Newtonsoft.Json;

namespace Khaos.MediatR.Rpc.Codecs.NewtosoftJson;

public class NewtosoftJsonStreamCodec : IStreamCodec
{
    private readonly JsonSerializerSettings? _settings;

    public NewtosoftJsonStreamCodec(JsonSerializerSettings? settings = default)
    {
        _settings = settings;
    }

    public IReadOnlySet<string> SupportedContentTypes => new HashSet<string> {"application/json"};
    public string OutputContentType => "application/json; charset=utf-8";
    
    public async ValueTask<object?> Decode(Type type, Stream stream, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var contentString = await reader.ReadToEndAsync();

        return JsonConvert.DeserializeObject(contentString, type, _settings);
    }

    public Task Encode(object? @object, Stream stream, CancellationToken cancellationToken)
    {
        var stringContent = JsonConvert.SerializeObject(@object, _settings);
        using var writer = new StreamWriter(stream, Encoding.UTF8);

        return writer.WriteAsync(stringContent);
    }
}