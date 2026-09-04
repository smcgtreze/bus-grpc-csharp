using System.Text;
using Transports;

public class LineServiceImpl : ServiceImpl<LineRequest, LineResponse>
{
    public override string GetEndpoint(LineRequest request) => "line";

    public override void AppendArgumentsToUrl(LineRequest request, StringBuilder url)
    {
        AppendQueryParam(url, "routeId", request.RouteId);
        AppendQueryParam(url, "countryIso", request.CountryIso);
        AppendQueryParam(url, "regionName", request.RegionName);
        AppendQueryParam(url, "lang", request.Lang);
        AppendQueryParam(url, "date", request.Date);
    }

    public override Transports.Key getKey(LineRequest request) => request.Key;
}