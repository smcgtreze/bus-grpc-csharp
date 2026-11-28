using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Transports;

internal static class Program
{
    private static readonly string[] Commands =
    [
        "alerts",
        "gtfs-feeds",
        "line",
        "next-departures",
        "pedestrian-matrix",
        "pedestrian-route",
        "vehicle-positions",
        "routes",
        "stops-in-radius",
        "trip"
    ];

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
        Console.WriteLine($"Commands: {string.Join(", ", Commands)}");
        Console.WriteLine("Enter 'exit' to quit.");

        while (true)
        {
            Console.Write("\nRPC: ");
            var command = Console.ReadLine()?.Trim().ToLowerInvariant();
            if (command is null || command == "exit")
                return 0;

            if (!Commands.Contains(command))
            {
                Console.Error.WriteLine("Unknown RPC. Choose one of the listed commands.");
                continue;
            }

            Console.Write("Request JSON: ");
            var json = Console.ReadLine();
            if (json is null)
                return 0;

            try
            {
                await InvokeAsync(channel, command, json);
            }
            catch (RpcException exception)
            {
                Console.Error.WriteLine($"gRPC error: {exception.StatusCode}: {exception.Status.Detail}");
            }
            catch (InvalidProtocolBufferException exception)
            {
                Console.Error.WriteLine($"Invalid request JSON: {exception.Message}");
            }
        }
    }

    private static Task InvokeAsync(GrpcChannel channel, string command, string json) =>
        command switch
        {
            "alerts" => SendAsync<AlertsRequest, AlertsResponse>(
                json, request => new AlertsService.AlertsServiceClient(channel).GetAlertsAsync(request).ResponseAsync),
            "gtfs-feeds" => SendAsync<GtfsFeedsCatalogRequest, GtfsFeedsCatalogResponse>(
                json, request => new GtfsFeedsCatalogService.GtfsFeedsCatalogServiceClient(channel)
                    .GetGtfsFeedsCatalogAsync(request).ResponseAsync),
            "line" => SendAsync<LineRequest, LineResponse>(
                json, request => new LineService.LineServiceClient(channel).GetLineAsync(request).ResponseAsync),
            "next-departures" => SendAsync<NextDeparturesRequest, NextDeparturesResponse>(
                json, request => new NextDeparturesService.NextDeparturesServiceClient(channel)
                    .GetNextDeparturesAsync(request).ResponseAsync),
            "pedestrian-matrix" => SendAsync<PedestrianMatrixRequest, PedestrianMatrixResponse>(
                json, request => new PedestrianMatrixService.PedestrianMatrixServiceClient(channel)
                    .GetPedestrianMatrixAsync(request).ResponseAsync),
            "pedestrian-route" => SendAsync<PedestrianRouteRequest, PedestrianRouteResponse>(
                json, request => new PedestrianRouteService.PedestrianRouteServiceClient(channel)
                    .GetPedestrianRouteAsync(request).ResponseAsync),
            "vehicle-positions" => SendAsync<VehiclePositionsRequest, VehiclePositionsResponse>(
                json, request => new VehiclePositionsService.VehiclePositionsServiceClient(channel)
                    .GetVehiclePositionsAsync(request).ResponseAsync),
            "routes" => SendAsync<RoutesRequest, RoutesResponse>(
                json, request => new RouteService.RouteServiceClient(channel).GetRoutesAsync(request).ResponseAsync),
            "stops-in-radius" => SendAsync<StopsInRadiusRequest, StopsInRadiusResponse>(
                json, request => new StopsInRadiusService.StopsInRadiusServiceClient(channel)
                    .GetStopsInRadiusAsync(request).ResponseAsync),
            "trip" => SendAsync<TripRequest, TripResponse>(
                json, request => new TripService.TripServiceClient(channel).GetTripAsync(request).ResponseAsync),
            _ => throw new ArgumentOutOfRangeException(nameof(command), command, "Unknown RPC.")
        };

    private static async Task SendAsync<TRequest, TResponse>(
        string json,
        Func<TRequest, Task<TResponse>> send)
        where TRequest : IMessage<TRequest>, new()
        where TResponse : IMessage<TResponse>, new()
    {
        var request = JsonParser.Default.Parse<TRequest>(json);
        var response = await send(request);
        Console.WriteLine(JsonFormatter.Default.Format(response));
    }
}
