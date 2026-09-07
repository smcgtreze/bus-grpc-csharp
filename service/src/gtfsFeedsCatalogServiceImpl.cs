using System.Text;
using Transports;

namespace BusGrpc.Services;

public class GtfsFeedsCatalogServiceImpl : ServiceImpl<GtfsFeedsCatalogRequest, GtfsFeedsCatalogResponse>
{
    public override string GetEndpoint(GtfsFeedsCatalogRequest request) => "getGtfsFeedsDownloads";

    public override void AppendArgumentsToUrl(GtfsFeedsCatalogRequest request, StringBuilder url)
    {
        AppendQueryParam(url, "countryIso", request.CountryIso);
        AppendQueryParam(url, "feedName", request.FeedName);
    }

    public override Transports.Key GetKey(GtfsFeedsCatalogRequest request) => request.Key;
}