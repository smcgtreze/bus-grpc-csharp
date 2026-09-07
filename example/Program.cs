using BusGrpc.Services;
using Google.Protobuf;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Transports;

namespace MapClickDemo
{
    internal static class Program
    {
        private static NextDeparturesServiceImpl departuresService = new NextDeparturesServiceImpl();

        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();
            var form = new Form1(OnLocationPicked);
            Application.Run(form);
        }

        private static async Task OnLocationPicked(string location)
        {
            try
            {
                var request = new NextDeparturesRequest
                {
                    Location = location,
                    CountryIso = "PRT",
                    Key = new Key
                    {
                        CapiHost = "busmaps.com",
                        ApiKey = "825858535684745631cd5fef5c1626ee",
                        Port = 8443
                    }
                };
                Console.WriteLine("Picked location " + request.Location.ToString() + " in " + request.CountryIso);
                var response = await departuresService.GetService(request);

                Console.WriteLine(formatResponse(response));
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Location: {location}\n\nError:\n{ex.Message}",
                    "Caught Exception"
                );
            }
        }

        private static string formatResponse(NextDeparturesResponse response)
        {
            string json = JsonFormatter.Default.Format(response);

            string pretty = JsonConvert.SerializeObject(
                JsonConvert.DeserializeObject(json),
                Formatting.Indented
            );
            return pretty;
        }
    }
}