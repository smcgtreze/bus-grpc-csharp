using System.Text;
using Transports;

public class GtfsFeedsCatalogServiceImpl : ServiceImpl<GtfsFeedsCatalogRequest, GtfsFeedsCatalogResponse>
{
    public override string GetEndpoint(GtfsFeedsCatalogRequest request) => "getGtfsFeedsDownloads";

    public override void AppendArgumentsToUrl(GtfsFeedsCatalogRequest request, StringBuilder url)
    {
        AppendQueryParam(url, "countryIso", request.CountryIso);
        AppendQueryParam(url, "feedName", request.FeedName);
    }

    public override Transports.Key getKey(GtfsFeedsCatalogRequest request) => request.Key;
}