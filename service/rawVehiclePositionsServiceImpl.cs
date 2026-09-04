using System.Text;
using Transports;

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
    }

    public override Transports.Key getKey(VehiclePositionsRequest request) => request.Key;
}