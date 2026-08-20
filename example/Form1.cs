using Microsoft.Web.WebView2.Core;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace MapClickDemo
{
    public partial class Form1 : Form
    {
        private readonly Func<string, Task> locationCallback;

        public Form1(Func<string, Task> locationCallback)
        {
            InitializeComponent();
            this.locationCallback = locationCallback;
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await webView2.EnsureCoreWebView2Async(null);

            string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "map.html");
            webView2.Source = new Uri(htmlPath);

            webView2.CoreWebView2.WebMessageReceived += WebMessageReceived;
        }
        private async void WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string json = e.WebMessageAsJson;
            dynamic msg = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            string location =
                $"{msg.lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
                $"{msg.lng.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
            
            await locationCallback(location);
        }
    }
}
