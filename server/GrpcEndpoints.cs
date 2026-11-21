using BusGrpc.Services;
using Google.Protobuf;
using Grpc.Core;
using Transports;

namespace BusGrpc.Server;

internal static class GrpcForwarder
{
    public static Task<TResponse> SendAsync<TRequest, TResponse>(
        ServiceImpl<TRequest, TResponse> client,
        TRequest request,
        ServerCallContext context)
        where TRequest : IMessage, new()
        where TResponse : IMessage, new()
    {
        var key = client.GetKey(request);
        if (key is null || string.IsNullOrWhiteSpace(key.ApiKey) ||
            string.IsNullOrWhiteSpace(key.CapiHost) || key.Port is < 1 or > 65535)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument,
                "Provide key.apiKey, key.capiHost and key.port (1-65535)."));
        }

        return client.GetService(request, context.CancellationToken);
    }
}

public sealed class AlertsEndpoint(AlertsServiceImpl client) : AlertsService.AlertsServiceBase
{
    public override Task<AlertsResponse> GetAlerts(AlertsRequest request, ServerCallContext context) =>
        GrpcForwarder.SendAsync(client, request, context);
}

public sealed class GtfsFeedsCatalogEndpoint(GtfsFeedsCatalogServiceImpl client)
    : GtfsFeedsCatalogService.GtfsFeedsCatalogServiceBase
{
    public override Task<GtfsFeedsCatalogResponse> GetGtfsFeedsCatalog(
        GtfsFeedsCatalogRequest request, ServerCallContext context) =>
        GrpcForwarder.SendAsync(client, request, context);
}

public sealed class LineEndpoint(LineServiceImpl client) : LineService.LineServiceBase
{
    public override Task<LineResponse> GetLine(LineRequest request, ServerCallContext context) =>
        GrpcForwarder.SendAsync(client, request, context);
}

public sealed class NextDeparturesEndpoint(NextDeparturesServiceImpl client)
    : NextDeparturesService.NextDeparturesServiceBase
{
    public override Task<NextDeparturesResponse> GetNextDepartures(
        NextDeparturesRequest request, ServerCallContext context) =>
        GrpcForwarder.SendAsync(client, request, context);
}

public sealed class PedestrianMatrixEndpoint(PedestrianMatrixServiceImpl client)
    : PedestrianMatrixService.PedestrianMatrixServiceBase
{
    public override Task<PedestrianMatrixResponse> GetPedestrianMatrix(
        PedestrianMatrixRequest request, ServerCallContext context) =>
        GrpcForwarder.SendAsync(client, request, context);
}

public sealed class PedestrianRouteEndpoint(PedestrianRouteServiceImpl client)
    : PedestrianRouteService.PedestrianRouteServiceBase
{
    public override Task<PedestrianRouteResponse> GetPedestrianRoute(
        PedestrianRouteRequest request, ServerCallContext context) =>
        GrpcForwarder.SendAsync(client, request, context);
}

public sealed class VehiclePositionsEndpoint(RawVehiclePositionsServiceImpl client)
    : VehiclePositionsService.VehiclePositionsServiceBase
{
    public override Task<VehiclePositionsResponse> GetVehiclePositions(
        VehiclePositionsRequest request, ServerCallContext context) =>
        GrpcForwarder.SendAsync(client, request, context);
}

public sealed class RoutesEndpoint(RouteServiceImpl client) : RouteService.RouteServiceBase
{
    public override Task<RoutesResponse> GetRoutes(RoutesRequest request, ServerCallContext context) =>
        GrpcForwarder.SendAsync(client, request, context);
}

public sealed class StopsInRadiusEndpoint(StopsInRadiusServiceImpl client)
    : StopsInRadiusService.StopsInRadiusServiceBase
{
    public override Task<StopsInRadiusResponse> GetStopsInRadius(
        StopsInRadiusRequest request, ServerCallContext context) =>
        GrpcForwarder.SendAsync(client, request, context);
}

public sealed class TripEndpoint(TripServiceImpl client) : TripService.TripServiceBase
{
    public override Task<TripResponse> GetTrip(TripRequest request, ServerCallContext context) =>
        GrpcForwarder.SendAsync(client, request, context);
}
