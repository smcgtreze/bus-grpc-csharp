using BusGrpc.Server;
using BusGrpc.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5000");
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureEndpointDefaults(endpoint => endpoint.Protocols = HttpProtocols.Http2);
});

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddSingleton<AlertsServiceImpl>();
builder.Services.AddSingleton<GtfsFeedsCatalogServiceImpl>();
builder.Services.AddSingleton<LineServiceImpl>();
builder.Services.AddSingleton<NextDeparturesServiceImpl>();
builder.Services.AddSingleton<PedestrianMatrixServiceImpl>();
builder.Services.AddSingleton<PedestrianRouteServiceImpl>();
builder.Services.AddSingleton<RawVehiclePositionsServiceImpl>();
builder.Services.AddSingleton<RouteServiceImpl>();
builder.Services.AddSingleton<StopsInRadiusServiceImpl>();
builder.Services.AddSingleton<TripServiceImpl>();

var app = builder.Build();

app.MapGrpcService<AlertsEndpoint>();
app.MapGrpcService<GtfsFeedsCatalogEndpoint>();
app.MapGrpcService<LineEndpoint>();
app.MapGrpcService<NextDeparturesEndpoint>();
app.MapGrpcService<PedestrianMatrixEndpoint>();
app.MapGrpcService<PedestrianRouteEndpoint>();
app.MapGrpcService<VehiclePositionsEndpoint>();
app.MapGrpcService<RoutesEndpoint>();
app.MapGrpcService<StopsInRadiusEndpoint>();
app.MapGrpcService<TripEndpoint>();
app.MapGrpcReflectionService();

app.Run();
