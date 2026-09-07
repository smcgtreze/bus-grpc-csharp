using System.Text;
using Transports;

namespace BusGrpc.Services;

public class RouteServiceImpl : ServiceImpl<RoutesRequest, RoutesResponse>
{
    public override string GetEndpoint(RoutesRequest request) => "routes";

    public override void AppendArgumentsToUrl(RoutesRequest request, StringBuilder url){
        AppendQueryParam(url, "origin", request.Origin);
        AppendQueryParam(url, "destination", request.Destination);
        AppendQueryParam(url, "arrivalTime", request.ArrivalTime);
        AppendQueryParam(url, "departureTime", request.DepartureTime);
        AppendQueryParam(url, "transfers", request.Transfers);
        AppendQueryParam(url, "geometry", request.Geometry);
        AppendQueryParam(url, "transport", request.Transport);
        AppendQueryParam(url, "maxRoutes", request.MaxRoutes);
        AppendQueryParam(url, "lang", request.Lang);
        AppendQueryParam(url, "barrierMode", request.BarrierMode);
    }

    public override Transports.Key GetKey(RoutesRequest request) => request.Key;
}
