using System.Text;
using Transports;

namespace BusGrpc.Services;

public class PedestrianRouteServiceImpl : ServiceImpl<PedestrianRouteRequest, PedestrianRouteResponse>
{
    public override string GetEndpoint(PedestrianRouteRequest request) => "pedestrian/route";

    public override void AppendArgumentsToUrl(PedestrianRouteRequest request, StringBuilder url)
    {
        AppendQueryParam(url, "alternatives", request.Alternatives == 0 ? null : request.Alternatives);
        AppendQueryParam(url, "overview", request.Overview);
        AppendQueryParam(url, "geometries", request.Geometries);
        AppendQueryParam(url, "steps", request.Steps);
        AppendQueryParam(url, "annotations", request.Annotations);
        AppendQueryParam(url, "skip_waypoints", request.SkipWaypoints);
        AppendQueryParam(url, "regionName", request.RegionName);
        AppendQueryParam(url, "lang", request.Lang);
        AppendQueryParam(url, "coordinates", request.Coordinates);
    }

    public override Transports.Key GetKey(PedestrianRouteRequest request) => request.Key;
}