using BusGrpc.Services;
using Google.Protobuf;
using Newtonsoft.Json;
using System.Diagnostics;
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
                var response = await MeasureElapsedTime(async () => await departuresService.GetService(request), out long elapsedMs);
                
                Console.WriteLine($"Next departures handler returned {response.GetType().Name} with {response.StopDepartures.Count} stops in {elapsedMs} ms.");
                PrintResponse(response);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Location: {location}\n\nError:\n{ex.Message}",
                    "Caught Exception"
                );
            }
        }

        private static string FormatResponse(NextDeparturesResponse response)
        {
            string json = JsonFormatter.Default.Format(response);

            string pretty = JsonConvert.SerializeObject(
                JsonConvert.DeserializeObject(json),
                Formatting.Indented
            );
            return pretty;
        }

        private static T MeasureElapsedTime<T>(Func<T> predicate, out long elapsedMilliseconds)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            T response = predicate();
            stopwatch.Stop();
            elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            
            return response;
        }
        
        static void PrintResponse(NextDeparturesResponse response)
        {
            foreach (var stop in response.StopDepartures)
            {
                Console.WriteLine($"Stop: {stop.StopName} ({stop.StopId})");
                foreach (var departure in stop.DepartureList)
                {
                    Console.WriteLine($"  - {departure.DepartureTime} | {departure.RouteShortName} | {departure.TripHeadsign}");
                }
            }
        }
    }
}