using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Khaos.MediatR.Rpc.Codecs.NewtosoftJson;

public class NewtosoftJsonStreamCodec : IStreamCodec
{
    private readonly JsonSerializerSettings _options;

    public NewtosoftJsonStreamCodec(JsonSerializerSettings? options = default)
    {
        _options = options ?? DefaultOptions;
    }

    public IReadOnlySet<string> SupportedContentTypes => new HashSet<string> {"application/json"};
    public string OutputContentType => "application/json; charset=utf-8";
    
    public async ValueTask<object?> Decode(Type type, Stream stream, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var contentString = await reader.ReadToEndAsync();

        return JsonConvert.DeserializeObject(contentString, type, _options);
    }

    public async Task Encode(object? @object, Stream stream, CancellationToken cancellationToken)
    {
        var stringContent = JsonConvert.SerializeObject(@object, _options);
        await using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

        await writer.WriteAsync(stringContent);
    }

    public static JsonSerializerSettings DefaultOptions = new()
    {
        ContractResolver =
            new CamelCasePropertyNamesContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy {ProcessDictionaryKeys = false}
            },
        NullValueHandling = NullValueHandling.Ignore,
        Converters = new List<JsonConverter>
        {
            new StringEnumConverter(new CamelCaseNamingStrategy {ProcessDictionaryKeys = false})
        },
        Formatting = Formatting.None
    };
}