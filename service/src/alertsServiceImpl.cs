using System.Text;
using Transports;

namespace BusGrpc.Services;

public class AlertsServiceImpl : ServiceImpl<AlertsRequest, AlertsResponse>
{
    public override string GetEndpoint(AlertsRequest request) => "alerts";

    public override void AppendArgumentsToUrl(AlertsRequest request, StringBuilder url)
    {
        AppendQueryParam(url, "regionName", request.RegionName);
        AppendQueryParam(url, "countryIso", request.CountryIso);
        AppendQueryParam(url, "routeId", request.RouteId);
        AppendQueryParam(url, "operatorId", request.OperatorId);
        AppendQueryParam(url, "lang", request.Lang);
    }

    public override Transports.Key GetKey(AlertsRequest request) => request.Key;
}