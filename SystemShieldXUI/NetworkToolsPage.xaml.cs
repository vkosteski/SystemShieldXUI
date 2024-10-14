using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;

namespace SystemShieldXUI
{
    public partial class NetworkToolsPage : Page
    {
        public NetworkToolsPage()
        {
            InitializeComponent();
            LoadNetworkInfo();
        }

        private async void LoadNetworkInfo()
        {
            await LoadPublicIpAsync();
        }

        private async Task LoadPublicIpAsync()
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("SystemShieldX/1.0");

                string ipResponse = await client.GetStringAsync("https://api.myip.com/");
                var ipJson = JsonDocument.Parse(ipResponse).RootElement;
                string ip = ipJson.GetProperty("ip").GetString();

                IpText.Text = $"IP Address: {ip}";

                // Optional geolocation
                try
                {
                    string geoResponse = await client.GetStringAsync($"https://ipapi.co/{ip}/json/");
                    var geoJson = JsonDocument.Parse(geoResponse).RootElement;

                    string city = geoJson.TryGetProperty("city", out var cityProp) ? cityProp.GetString() : "Unknown";
                    string country = geoJson.TryGetProperty("country_name", out var countryProp) ? countryProp.GetString() : "Unknown";
                    string isp = geoJson.TryGetProperty("org", out var ispProp) ? ispProp.GetString() : "Unknown";

                    LocationText.Text = $"Location: {city}, {country}";
                    IspText.Text = $"ISP: {isp}";
                }
                catch
                {
                    LocationText.Text = "Location data unavailable.";
                    IspText.Text = "";
                }
            }
            catch (Exception ex)
            {
                IpText.Text = "Failed to retrieve IP info.";
                LocationText.Text = ex.Message;
                IspText.Text = "";
            }
        }

        private async void ResolveDomain_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string domain = DomainInput.Text.Trim();
                if (string.IsNullOrWhiteSpace(domain))
                {
                    ResolvedIpText.Text = "Enter a domain first.";
                    return;
                }

                var host = await Dns.GetHostEntryAsync(domain);
                var ips = host.AddressList
                    .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                    .Select(ip => ip.ToString());

                ResolvedIpText.Text = $"Resolved IPs: {string.Join(", ", ips)}";
            }
            catch (Exception ex)
            {
                ResolvedIpText.Text = $"Error: {ex.Message}";
            }
        }

        private async void ReverseLookup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ip = ReverseInput.Text.Trim();
                var host = await Dns.GetHostEntryAsync(ip);
                ReverseResult.Text = $"Host: {host.HostName}";
            }
            catch (Exception ex)
            {
                ReverseResult.Text = $"Error: {ex.Message}";
            }
        }

        private async void TraceRoute_Click(object sender, RoutedEventArgs e)
        {
            string target = TraceInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(target))
            {
                TraceOutput.Text = "Enter a domain or IP.";
                return;
            }

            TraceOutput.Text = "Tracing...";

            await Task.Run(() =>
            {
                try
                {
                    var psi = new ProcessStartInfo("tracert", target)
                    {
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    var proc = Process.Start(psi);
                    string result = proc.StandardOutput.ReadToEnd();
                    proc.WaitForExit();

                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        TraceOutput.Text = result;
                    });
                }
                catch (Exception ex)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        TraceOutput.Text = $"Error: {ex.Message}";
                    });
                }
            });
        }

        private void ScanPorts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenPortsList.Items.Clear();

                var props = IPGlobalProperties.GetIPGlobalProperties();
                var listeners = props.GetActiveTcpListeners();

                foreach (var ep in listeners)
                {
                    OpenPortsList.Items.Add($"Listening: {ep.Address}:{ep.Port}");
                }

                if (!listeners.Any())
                    OpenPortsList.Items.Add("No open ports found.");
            }
            catch (Exception ex)
            {
                OpenPortsList.Items.Add($"Error: {ex.Message}");
            }
        }
    }
}
