using System.Text;
using Transports;

namespace BusGrpc.Services;

public class StopsInRadiusServiceImpl : ServiceImpl<StopsInRadiusRequest, StopsInRadiusResponse>
{
    public override string GetEndpoint(StopsInRadiusRequest request) => "stopsInRadius";

    public override void AppendArgumentsToUrl(StopsInRadiusRequest request, StringBuilder url)
    {
        AppendQueryParam(url, "location", request.Location);
        AppendQueryParam(url, "radius", request.Radius);
        AppendQueryParam(url, "limit", request.Limit);
        AppendQueryParam(url, "lang", request.Lang);
    }

    public override Transports.Key GetKey(StopsInRadiusRequest request) => request.Key;
}