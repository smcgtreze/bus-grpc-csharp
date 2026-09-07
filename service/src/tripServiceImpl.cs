using System.Text;
using Transports;

namespace BusGrpc.Services;

public class TripServiceImpl : ServiceImpl<TripRequest, TripResponse>
{
    public override string GetEndpoint(TripRequest request) => "trip";

    public override void AppendArgumentsToUrl(TripRequest request, StringBuilder url)
    {
        AppendQueryParam(url, "tripId", request.TripId);
        AppendQueryParam(url, "regionName", request.RegionName);
        AppendQueryParam(url, "lang", request.Lang);
    }

    public override Transports.Key GetKey(TripRequest request) => request.Key;
}