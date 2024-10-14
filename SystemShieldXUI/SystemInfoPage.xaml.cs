using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows.Controls;

namespace SystemShieldXUI
{
    public partial class SystemInfoPage : Page
    {
        public SystemInfoPage()
        {
            InitializeComponent();
            LoadSystemInfo();
        }

        private void LoadSystemInfo()
        {
            OsText.Text = $"{RuntimeInformation.OSDescription}";
            ArchText.Text = RuntimeInformation.OSArchitecture.ToString();
            MachineText.Text = Environment.MachineName;

            CpuText.Text = GetCpuName();
            CpuCoresText.Text = $"{Environment.ProcessorCount} logical cores";
            RamText.Text = GetInstalledRAM();
            GpuText.Text = GetGpuName();
            UptimeText.Text = GetSystemUptime();
        }

        private string GetCpuName()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("select Name from Win32_Processor");
                foreach (var obj in searcher.Get())
                {
                    return obj["Name"].ToString();
                }
            }
            catch { }
            return "Unknown";
        }

        private string GetInstalledRAM()
        {
            try
            {
                var ramBytes = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
                double ramGB = ramBytes / 1024.0 / 1024.0 / 1024.0;
                return $"{ramGB:F2} GB";
            }
            catch { return "Unknown"; }
        }

        private string GetGpuName()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("select Name from Win32_VideoController");
                var gpus = searcher.Get().Cast<ManagementObject>().Select(mo => mo["Name"].ToString());
                return string.Join(", ", gpus.Distinct());
            }
            catch { return "Unknown"; }
        }

        private string GetSystemUptime()
        {
            try
            {
                var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
                return $"{(int)uptime.TotalDays}d {uptime.Hours}h {uptime.Minutes}m";
            }
            catch { return "Unknown"; }
        }
    }
}
