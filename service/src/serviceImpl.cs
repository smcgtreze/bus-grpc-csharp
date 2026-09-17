using System.Text;
using Google.Protobuf;
using Transports;

namespace BusGrpc.Services;

public abstract class ServiceImpl<IN, OUT>
    where IN : IMessage, new()
    where OUT : IMessage, new()
{
    private readonly HttpClient httpClient;

    protected ServiceImpl(HttpClient? httpClient = null)
    {
        this.httpClient = httpClient ?? new HttpClient();
    }

    public abstract string GetEndpoint(IN request);
    public abstract void AppendArgumentsToUrl(IN request, StringBuilder url);
    public abstract Transports.Key GetKey(IN request);
    
    public async Task<OUT> GetService(
        IN request,
        CancellationToken cancellationToken = default)
    {
        var key = GetKey(request);
        var url = new StringBuilder($"https://capi.{key.CapiHost}:{key.Port}/{GetEndpoint(request)}");

        AppendArgumentsToUrl(request, url);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url.ToString());
        httpRequest.Headers.Add("capi-key", $"Bearer {key.ApiKey}");
        httpRequest.Headers.Add("capi-host", key.CapiHost);

        using var response = await httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        return ProtoJson.Tolerant.Parse<OUT>(body);
    }

    public static void AppendQueryParam(StringBuilder builder, string name, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        builder.Append(builder.ToString().Contains("?") ? '&' : '?');
        builder.Append(name).Append('=').Append(Uri.EscapeDataString(value));
    }

    public static void AppendQueryParam(StringBuilder builder, string name, int? value)
    {
        if (!value.HasValue)
            return;

        builder.Append(builder.ToString().Contains("?") ? '&' : '?');
        builder.Append(name).Append('=').Append(Uri.EscapeDataString(value.Value.ToString()));
    }

    public static void AppendQueryParam(StringBuilder builder, string name, bool value)
    {
        if (!value)
            return;

        builder.Append(builder.ToString().Contains("?") ? '&' : '?');
        builder.Append(name).Append('=').Append(value.ToString().ToLowerInvariant());
    }

    private static class ProtoJson
    {
        public static readonly JsonParser Tolerant =
            new JsonParser(JsonParser.Settings.Default.WithIgnoreUnknownFields(true));
    }
}
