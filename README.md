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

## Geoapify address lookup

`GeoapifyGeocoder.GeocodeAddressAsync` takes an `HttpClient`, an address, and a
Geoapify API key, then returns latitude and longitude. Get a key by creating a
project in [Geoapify MyProjects](https://myprojects.geoapify.com/) and copying
its key from **API Keys**. Distinct from BusMaps API key.

```csharp
using BusGrpc.Client;

using var httpClient = new HttpClient();
var apiKey = Environment.GetEnvironmentVariable("GEOAPIFY_API_KEY")
    ?? throw new InvalidOperationException("Set GEOAPIFY_API_KEY.");
GeoCoordinates coordinates = await GeoapifyGeocoder.GeocodeAddressAsync(
    httpClient, "11 Avenue de la Bourdonnais, Paris, France", apiKey);
```

## Run only the gRPC server

The separate [server application](../server/Program.cs) requires the .NET 8 SDK
or newer.
From the repository root:

```powershell
dotnet run --project .\server\BusGrpc.Server.csproj
```

It listens on `localhost:5000` using plaintext HTTP/2. All ten services from the
[protobuf definitions](../proto) are exposed, with server reflection enabled so
Evans does not need local `.proto` files. This is a local development host, not a
TLS-enabled or authenticated public server. Stop it with Ctrl+C.

## Use the CLI client

Start the server, then run the console client in another terminal:

```powershell
dotnet run --project .\client\BusGrpc.Client.csproj
```

Pass a different server address as the first argument if needed. Choose an RPC
from the list and answer each request-field prompt in turn. Nested fields such
as `key` are prompted individually, and repeated fields accept values one at a
time; press Enter on a repeated-field prompt when you are done. The response is
printed as indented JSON. For example, for `next-departures`, enter:

```text
NextDeparturesRequest.location: 38.7223,-9.1393
NextDeparturesRequest.stopId:
NextDeparturesRequest.regionName:
NextDeparturesRequest.key.apiKey: your-api-key
NextDeparturesRequest.key.capiHost: busmaps.com
NextDeparturesRequest.key.port: 8443
...
```

Enter `exit` at the RPC prompt to quit. The client includes commands for all
ten exposed RPCs; request fields are defined in the [protobuf files](../proto).
Press Enter on a scalar prompt to leave its default value unchanged.

For the `routes` command, the `RoutesRequest.key` values are read from
`client\appsettings.local.json` rather than prompted for. Create that local file
with this structure and fill in your own key:

```json
{
  "routesKey": {
    "apiKey": "your-api-key",
    "capiHost": "busmaps.com",
    "port": 8443
  }
}
Find your key here https://busmaps.com/en/developers/profile#api
```

The local config is ignored by Git and is copied to the build output for local
runs. Other RPC commands continue to prompt for their request fields.

In another terminal:

```powershell
evans --host localhost --port 5000 --reflection repl
```

Inside Evans:

```text
show package
package transports
show service
service NextDeparturesService
show rpc
call GetNextDepartures
```

Answer the prompts using the request fields. For example, a next-departures
request uses `location` = `38.7223,-9.1393`, `countryIso` = `PRT`, and a nested
`key` containing `apiKey` = your API key, `capiHost` = `busmaps.com`, and
`port` = `8443`. The server forwards requests to the existing BusMaps HTTP
clients, so successful queries require valid upstream credentials and network
access. Missing or incomplete keys return gRPC `InvalidArgument`.

Use `show service`, then `service <ServiceName>` and `show rpc` to select any
other endpoint. The original map application still starts with
`dotnet run --project .\gtfs.csproj`.
