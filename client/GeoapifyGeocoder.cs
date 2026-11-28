using System.Text.Json;

namespace BusGrpc.Client;

public readonly record struct GeoCoordinates(double Latitude, double Longitude);

public static class GeoapifyGeocoder
{
    private const string SearchEndpoint = "https://api.geoapify.com/v1/geocode/search";

    public static async Task<GeoCoordinates> GeocodeAddressAsync(
        HttpClient httpClient,
        string address,
        string apiKey,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentException.ThrowIfNullOrWhiteSpace(address);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);

        var requestUri = $"{SearchEndpoint}?text={Uri.EscapeDataString(address)}&apiKey={Uri.EscapeDataString(apiKey)}";
        using var response = await httpClient.GetAsync(
            requestUri,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(
            responseStream,
            cancellationToken: cancellationToken);

        var root = document.RootElement;
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty("features", out var features) ||
            features.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidDataException("Geoapify returned a response without a features array.");
        }

        if (features.GetArrayLength() == 0)
            throw new InvalidOperationException("Geoapify found no results for the supplied address.");

        var firstFeature = features[0];
        if (firstFeature.ValueKind != JsonValueKind.Object ||
            !firstFeature.TryGetProperty("properties", out var properties) ||
            properties.ValueKind != JsonValueKind.Object ||
            !properties.TryGetProperty("lat", out var latitudeValue) ||
            !properties.TryGetProperty("lon", out var longitudeValue) ||
            latitudeValue.ValueKind != JsonValueKind.Number ||
            longitudeValue.ValueKind != JsonValueKind.Number)
        {
            throw new InvalidDataException("Geoapify's first result did not contain numeric latitude and longitude.");
        }

        var latitude = latitudeValue.GetDouble();
        var longitude = longitudeValue.GetDouble();
        if (latitude is < -90 or > 90 || longitude is < -180 or > 180)
            throw new InvalidDataException("Geoapify returned coordinates outside the valid geographic range.");

        return new GeoCoordinates(latitude, longitude);
    }
}
