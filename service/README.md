# BusGrpc.Services

A plug-and-play .NET library for calling the BusMaps GTFS services.

## Install from a project reference

```xml
<ProjectReference Include="..\service\BusGrpc.Services.csproj" />
```

For a published package, pack this project and add the resulting `BusGrpc.Services` package instead.

## Use from an application

```csharp
using BusGrpc.Services;
using Transports;

var departures = new NextDeparturesServiceImpl();

var response = await departures.GetService(new NextDeparturesRequest
{
    Location = "38.7223,-9.1393",
    CountryIso = "PRT",
    Key = new Key
    {
        CapiHost = "busmaps.com",
        ApiKey = "your-api-key",
        Port = 8443
    }
});
```

All service classes accept an optional `HttpClient`, so applications can provide a configured or test client:

```csharp
var departures = new NextDeparturesServiceImpl(httpClient);
```

The public `IBusService<TRequest, TResponse>` interface can be used by application code when it should not depend on a concrete implementation. The library contains clients for alerts, GTFS feeds, lines, next departures, pedestrian matrix and route, raw vehicle positions, routes, stops in radius, and trips.
