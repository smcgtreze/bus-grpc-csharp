using System.Text;
using Transports;

namespace BusGrpc.Services;

public class RawVehiclePositionsServiceImpl : ServiceImpl<VehiclePositionsRequest, VehiclePositionsResponse>
{
    public override string GetEndpoint(VehiclePositionsRequest request) => "rawVehiclePositions";

    public override void AppendArgumentsToUrl(VehiclePositionsRequest request, StringBuilder url)
    {
        AppendQueryParam(url, "bounding_box", request.BoundingBox);
        AppendQueryParam(url, "regionName", request.RegionName);
        AppendQueryParam(url, "routeId", request.RouteId);
        AppendQueryParam(url, "countryIso", request.CountryIso);
        AppendQueryParam(url, "lang", request.Lang);
        AppendQueryParam(url, "tripId", request.TripId);
    }

    public override Transports.Key GetKey(VehiclePositionsRequest request) => request.Key;
}