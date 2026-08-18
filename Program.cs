using Transports;

class Program
{
    static async Task Main(string[] args)
    {
        var service = new NextDeparturesServiceImpl();
        var request = new NextDeparturesRequest
        {
            Location = "51.537511,-0.152208",
            CountryIso = "GBR",
            Key = new Key
            {
                CapiHost = "busmaps.com",
                ApiKey = "825858535684745631cd5fef5c1626ee",
                Port = 8443
            }
        };

        var response = await service.GetService(request, null);

        Console.WriteLine(response);
        MessageBox.Show(response.ToString(), " Response");
    }
}
