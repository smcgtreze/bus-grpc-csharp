using System.Text;
using Transports;

public class StopsInRadiusServiceImpl : ServiceImpl<StopsInRadiusRequest, StopsInRadiusResponse>
{
    public override string GetEndpoint(StopsInRadiusRequest request) => "stopsInRadius";

    public override void AppendArgumentsToUrl(StopsInRadiusRequest request, StringBuilder url)
    {
        AppendQueryParam(url, "location", request.Location);
        AppendQueryParam(url, "radius", request.Radius);
        AppendQueryParam(url, "limit", request.Limit);
    }

    public override Transports.Key getKey(StopsInRadiusRequest request) => request.Key;
}