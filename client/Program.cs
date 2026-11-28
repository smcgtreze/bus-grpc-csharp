using System.Collections;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Grpc.Core;
using Grpc.Net.Client;
using Transports;

internal static class Program
{
    private enum CommandsEnum
    {
        Alerts,
        GtfsFeeds,
        Line,
        NextDepartures,
        PedestrianMatrix,
        PedestrianRoute,
        VehiclePositions,
        Routes,
        StopsInRadius,
        Trip
    }

    private static readonly Dictionary<string, CommandsEnum> CommandsMap = new Dictionary<string, CommandsEnum>(StringComparer.OrdinalIgnoreCase)
    {
        { "alerts", CommandsEnum.Alerts },
        { "gtfs-feeds", CommandsEnum.GtfsFeeds },
        { "line", CommandsEnum.Line },
        { "next-departures", CommandsEnum.NextDepartures },
        { "pedestrian-matrix", CommandsEnum.PedestrianMatrix },
        { "pedestrian-route", CommandsEnum.PedestrianRoute },
        { "vehicle-positions", CommandsEnum.VehiclePositions },
        { "routes", CommandsEnum.Routes },
        { "stops-in-radius", CommandsEnum.StopsInRadius },
        { "trip", CommandsEnum.Trip }
    };

    private sealed class ClientConfig
    {
        [JsonPropertyName("routesKey")]
        public KeyConfig? RoutesKey { get; set; }
    }

    private sealed class KeyConfig
    {
        [JsonPropertyName("apiKey")]
        public string? ApiKey { get; set; }

        [JsonPropertyName("capiHost")]
        public string? CapiHost { get; set; }

        [JsonPropertyName("port")]
        public int Port { get; set; }
    }

    private static async Task<int> Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.Error.WriteLine("Usage: BusGrpc.Client [server-address]");
            return 1;
        }

        var address = args.Length == 1 ? args[0] : "http://localhost:5000";
        if (!Uri.TryCreate(address, UriKind.Absolute, out var serverAddress) ||
            (serverAddress.Scheme != Uri.UriSchemeHttp && serverAddress.Scheme != Uri.UriSchemeHttps))
        {
            Console.Error.WriteLine("Server address must be an absolute http:// or https:// URL.");
            return 1;
        }

        using var channel = GrpcChannel.ForAddress(serverAddress);
        Console.WriteLine($"Connected to {serverAddress}");
        Console.WriteLine($"Commands: {string.Join(", ", CommandsMap.Keys)}");
        Console.WriteLine("Enter 'exit' to quit.");

        while (true)
        {
            Console.Write("\nRPC: ");
            var command = Console.ReadLine()?.Trim().ToLowerInvariant();
            if (command is null || command == "exit")
                return 0;

            if (!CommandsMap.ContainsKey(command))
            {
                Console.Error.WriteLine("Unknown RPC. Choose one of the listed commands.");
                continue;
            }

            try
            {
                await InvokeAsync(channel, CommandsMap[command]);
            }
            catch (Exception exception)
            {
                var exitCode = HandleException(exception);
                if (exitCode.HasValue)
                    return exitCode.Value;
            }
        }
    }

    private static int? HandleException(Exception exception)
    {
        switch (exception)
        {
            case EndOfStreamException:
                return 0;
            case RpcException rpcException:
                Console.Error.WriteLine(
                    $"gRPC error: {rpcException.StatusCode}: {rpcException.Status.Detail}");
                return null;
            case FormatException formatException:
                Console.Error.WriteLine($"Invalid request value: {formatException.Message}");
                return null;
            case OverflowException overflowException:
                Console.Error.WriteLine($"Request value is out of range: {overflowException.Message}");
                return null;
            case IOException ioException:
                Console.Error.WriteLine($"Local client configuration error: {ioException.Message}");
                return null;
            case JsonException jsonException:
                Console.Error.WriteLine($"Local client configuration is not valid JSON: {jsonException.Message}");
                return null;
            case UnauthorizedAccessException accessException:
                Console.Error.WriteLine($"Cannot read local client configuration: {accessException.Message}");
                return null;
            case InvalidDataException dataException:
                Console.Error.WriteLine($"Invalid local client configuration: {dataException.Message}");
                return null;
            default:
                Console.Error.WriteLine($"Unexpected error: {exception}");
                return 1;
        }
    }

    private static Task InvokeAsync(GrpcChannel channel, CommandsEnum command) =>
        command switch
        {
            CommandsEnum.Alerts => SendAsync<AlertsRequest, AlertsResponse>(
                request => new AlertsService.AlertsServiceClient(channel).GetAlertsAsync(request).ResponseAsync),
            CommandsEnum.GtfsFeeds => SendAsync<GtfsFeedsCatalogRequest, GtfsFeedsCatalogResponse>(
                request => new GtfsFeedsCatalogService.GtfsFeedsCatalogServiceClient(channel)
                    .GetGtfsFeedsCatalogAsync(request).ResponseAsync),
            CommandsEnum.Line => SendAsync<LineRequest, LineResponse>(
                request => new LineService.LineServiceClient(channel).GetLineAsync(request).ResponseAsync),
            CommandsEnum.NextDepartures => SendAsync<NextDeparturesRequest, NextDeparturesResponse>(
                request => new NextDeparturesService.NextDeparturesServiceClient(channel)
                    .GetNextDeparturesAsync(request).ResponseAsync),
            CommandsEnum.PedestrianMatrix => SendAsync<PedestrianMatrixRequest, PedestrianMatrixResponse>(
                request => new PedestrianMatrixService.PedestrianMatrixServiceClient(channel)
                    .GetPedestrianMatrixAsync(request).ResponseAsync),
            CommandsEnum.PedestrianRoute => SendAsync<PedestrianRouteRequest, PedestrianRouteResponse>(
                request => new PedestrianRouteService.PedestrianRouteServiceClient(channel)
                    .GetPedestrianRouteAsync(request).ResponseAsync),
            CommandsEnum.VehiclePositions => SendAsync<VehiclePositionsRequest, VehiclePositionsResponse>(
                request => new VehiclePositionsService.VehiclePositionsServiceClient(channel)
                    .GetVehiclePositionsAsync(request).ResponseAsync),
            CommandsEnum.Routes => SendAsync<RoutesRequest, RoutesResponse>(
                request => new RouteService.RouteServiceClient(channel).GetRoutesAsync(request).ResponseAsync,
                configureRequest: request => request.Key = ReadRoutesKeyFromConfig()),
            CommandsEnum.StopsInRadius => SendAsync<StopsInRadiusRequest, StopsInRadiusResponse>(
                request => new StopsInRadiusService.StopsInRadiusServiceClient(channel)
                    .GetStopsInRadiusAsync(request).ResponseAsync),
            CommandsEnum.Trip => SendAsync<TripRequest, TripResponse>(
                request => new TripService.TripServiceClient(channel).GetTripAsync(request).ResponseAsync),
            _ => throw new ArgumentOutOfRangeException(nameof(command), command, "Unknown RPC.")
        };

    private static async Task SendAsync<TRequest, TResponse>(
        Func<TRequest, Task<TResponse>> send,
        Action<TRequest>? configureRequest = null)
        where TRequest : IMessage<TRequest>, new()
        where TResponse : IMessage<TResponse>, new()
    {
        var request = new TRequest();
        configureRequest?.Invoke(request);
        PromptForMessage(request, request.Descriptor.Name);

        TResponse response = await send(request);
        using var document = JsonDocument.Parse(JsonFormatter.Default.Format(response));
        var formattedJson = JsonSerializer.Serialize(
            document.RootElement,
            new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine(formattedJson);
    }

    private static void PromptForMessage(IMessage message, string fieldPath)
    {
        foreach (var field in message.Descriptor.Fields.InFieldNumberOrder())
        {
            var prompt = $"{fieldPath}.{field.Name}";
            if (field.FieldType == FieldType.Message)
            {
                if (field.Accessor.HasValue(message))
                    continue;

                var nestedMessage = field.MessageType.Parser.ParseFrom(Array.Empty<byte>());
                PromptForMessage(nestedMessage, prompt);
                field.Accessor.SetValue(message, nestedMessage);
            }
            else if (field.IsRepeated)
            {
                var values = (IList)field.Accessor.GetValue(message);
                while (true)
                {
                    var input = ReadInput($"{prompt} (press Enter to finish): ");
                    if (string.IsNullOrEmpty(input))
                        break;

                    var value = TryParseValue(field, input);
                    if (value is not null)
                        values.Add(value);
                }
            }
            else
            {
                while (true)
                {
                    var input = ReadInput($"{prompt}: ");
                    if (string.IsNullOrEmpty(input))
                        break;

                    var value = TryParseValue(field, input);
                    if (value is not null)
                    {
                        field.Accessor.SetValue(message, value);
                        break;
                    }
                }
            }
        }
    }

    private static Key ReadRoutesKeyFromConfig()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "appsettings.local.json");
        var config = JsonSerializer.Deserialize<ClientConfig>(File.ReadAllText(path));
        var key = config?.RoutesKey;

        if (string.IsNullOrWhiteSpace(key?.ApiKey) ||
            string.IsNullOrWhiteSpace(key.CapiHost) ||
            key.Port <= 0)
        {
            throw new InvalidDataException(
                $"The routesKey configuration in '{path}' must include apiKey, capiHost, and a positive port.");
        }

        return new Key
        {
            ApiKey = key.ApiKey,
            CapiHost = key.CapiHost,
            Port = key.Port
        };
    }

    private static string ReadInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? throw new EndOfStreamException();
    }

    private static object? TryParseValue(FieldDescriptor field, string input)
    {
        try
        {
            return ParseValue(field, input);
        }
        catch (FormatException exception)
        {
            Console.Error.WriteLine(exception.Message);
            return null;
        }
    }

    private static object ParseValue(FieldDescriptor field, string input) =>
        field.FieldType switch
        {
            FieldType.String => input,
            FieldType.Bool when bool.TryParse(input, out var value) => value,
            FieldType.Int32 or FieldType.SInt32 or FieldType.SFixed32
                when int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) => value,
            FieldType.Int64 or FieldType.SInt64 or FieldType.SFixed64
                when long.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) => value,
            FieldType.UInt32 or FieldType.Fixed32
                when uint.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) => value,
            FieldType.UInt64 or FieldType.Fixed64
                when ulong.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) => value,
            FieldType.Float
                when float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) => value,
            FieldType.Double
                when double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) => value,
            _ => throw new FormatException($"'{input}' is not a valid {field.FieldType} for '{field.Name}'.")
        };
}
