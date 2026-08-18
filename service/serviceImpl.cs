using System.Text;
using Google.Protobuf;
using Grpc.Core;
using Transports;

public abstract class ServiceImpl<IN, OUT> where IN : IMessage, new() where OUT : IMessage, new()
{
    public abstract string GetEndpoint(IN request);
    public abstract void AppendArgumentsToUrl(IN request, StringBuilder url);
    public abstract Transports.Key getKey(IN request);
    
    public async Task<OUT> GetService(
        IN request,
        ServerCallContext context)
    {
        HttpClient httpClient = new HttpClient();
        var url = new StringBuilder($"https://capi.{getKey(request).CapiHost}:{getKey(request).Port}/{GetEndpoint(request)}");

        AppendArgumentsToUrl(request, url);

        var httpRequest = new HttpRequestMessage(HttpMethod.Get, url.ToString());
        httpRequest.Headers.Add("capi-key", $"Bearer {getKey(request).ApiKey}");
        httpRequest.Headers.Add("capi-host", getKey(request).CapiHost);

        var response = await httpClient.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
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

    private static class ProtoJson
    {
        public static readonly JsonParser Tolerant =
            new JsonParser(JsonParser.Settings.Default.WithIgnoreUnknownFields(true));
    }
}
