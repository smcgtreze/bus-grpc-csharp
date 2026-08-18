using System.Text;
using Transports;

public class NextDeparturesServiceImpl : ServiceImpl<NextDeparturesRequest, NextDeparturesResponse>
{
    public override string GetEndpoint(NextDeparturesRequest request)
    {
        return "nextDepartures";
    }

    public override void AppendArgumentsToUrl(NextDeparturesRequest request, StringBuilder url){
        AppendQueryParam(url, "location", request.Location);
        AppendQueryParam(url, "countryIso", request.CountryIso);
        AppendQueryParam(url, "stopId", request.StopId);
        AppendQueryParam(url, "regionName", request.RegionName);
        AppendQueryParam(url, "requestTime", request.RequestTime);
        AppendQueryParam(url, "radius", request.Radius);
        AppendQueryParam(url, "results", request.Results);
        AppendQueryParam(url, "barrierMode", request.BarrierMode);
        AppendQueryParam(url, "lang", request.Lang);
    }

    public override Transports.Key getKey(NextDeparturesRequest request)
    {
        return request.Key;
    }
}
