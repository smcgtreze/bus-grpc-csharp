using System.Text;
using Transports;

public class PedestrianMatrixServiceImpl : ServiceImpl<PedestrianMatrixRequest, PedestrianMatrixResponse>
{
    public override string GetEndpoint(PedestrianMatrixRequest request) => "pedestrian/matrix";

    public override void AppendArgumentsToUrl(PedestrianMatrixRequest request, StringBuilder url)
    {
        AppendQueryParam(url, "sourceLocations", string.Join(",", request.SourceLocations));
        AppendQueryParam(url, "destinationLocations", string.Join(",", request.DestinationLocations));
        AppendQueryParam(url, "annotations", request.Annotations);
    }

    public override Transports.Key getKey(PedestrianMatrixRequest request) => request.Key;
}