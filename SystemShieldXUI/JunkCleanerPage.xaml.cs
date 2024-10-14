using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace SystemShieldXUI
{
    public partial class OptimizerPage : Page
    {
        private List<string> junkFiles = new List<string>();
        private long totalSize = 0;

        // Recycle Bin API
        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        private static extern uint SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, RecycleFlags dwFlags);

        [Flags]
        private enum RecycleFlags : uint
        {
            SHERB_NOCONFIRMATION = 0x00000001,
            SHERB_NOPROGRESSUI = 0x00000002,
            SHERB_NOSOUND = 0x00000004
        }

        public OptimizerPage()
        {
            InitializeComponent();
        }

        private void ScanJunk_Click(object sender, RoutedEventArgs e)
        {
            junkFiles.Clear();
            JunkList.Items.Clear();
            totalSize = 0;

            var pathsToScan = new[]
            {
                Path.GetTempPath(),
                @"C:\Windows\Temp"
            };

            foreach (var path in pathsToScan)
            {
                if (Directory.Exists(path))
                {
                    try
                    {
                        var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            try
                            {
                                FileInfo fi = new FileInfo(file);
                                if (fi.Exists && fi.Length > 0)
                                {
                                    junkFiles.Add(file);
                                    totalSize += fi.Length;
                                    JunkList.Items.Add($"{file} ({FormatSize(fi.Length)})");
                                }
                            }
                            catch
                            {
                                // Skip files we can't access
                            }
                        }
                    }
                    catch
                    {
                        // Skip folders we can't access
                    }
                }
            }

            TotalSizeText.Text = $"Total Junk: {FormatSize(totalSize)}";
        }

        private void CleanJunk_Click(object sender, RoutedEventArgs e)
        {
            int deleted = 0;
            foreach (var file in junkFiles)
            {
                try
                {
                    File.Delete(file);
                    deleted++;
                }
                catch
                {
                    // Skip files we can't delete
                }
            }

            System.Windows.MessageBox.Show($"Deleted {deleted} junk files!", "Clean Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            JunkList.Items.Clear();
            TotalSizeText.Text = "Total Junk: 0 bytes";
            junkFiles.Clear();
        }

        private string FormatSize(long bytes)
        {
            if (bytes > 1_000_000_000)
                return $"{bytes / 1_000_000_000.0:F2} GB";
            else if (bytes > 1_000_000)
                return $"{bytes / 1_000_000.0:F2} MB";
            else if (bytes > 1_000)
                return $"{bytes / 1_000.0:F2} KB";
            else
                return $"{bytes} B";
        }

        private void ApplyTweaks_Click(object sender, RoutedEventArgs e)
        {
            int tweaksApplied = 0;       

            // 📡 Flush DNS Cache
            if (FlushDNSCheck.IsChecked == true)
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "ipconfig",
                        Arguments = "/flushdns",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    });
                    tweaksApplied++;
                }
                catch { }
            }

            if (OptimizeNetworkCheck.IsChecked == true)
            {
                try
                {
                    var qosKey = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\Psched");
                    qosKey?.SetValue("NonBestEffortLimit", 0, Microsoft.Win32.RegistryValueKind.DWord);
                    qosKey?.Close();
                    tweaksApplied++;
                }
                catch { }
            }

            // 🚀 Disable Startup Delay
            if (DisableStartupDelayCheck.IsChecked == true)
            {
                try
                {
                    var key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(
                        @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Serialize");
                    if (key != null)
                    {
                        key.SetValue("StartupDelayInMSec", 0, Microsoft.Win32.RegistryValueKind.DWord);
                        key.Close();
                        tweaksApplied++;
                    }
                }
                catch { }
            }

            if (BoostPerformanceCheck.IsChecked == true)
            {
                try
                {
                    // 🔥 Disable SysMain (Superfetch)
                    var disableSysMain = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "sc",
                        Arguments = "config SysMain start= disabled",
                        Verb = "runas",
                        CreateNoWindow = true,
                        UseShellExecute = true
                    };
                    System.Diagnostics.Process.Start(disableSysMain);

                    var stopSysMain = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "sc",
                        Arguments = "stop SysMain",
                        Verb = "runas",
                        CreateNoWindow = true,
                        UseShellExecute = true
                    };
                    System.Diagnostics.Process.Start(stopSysMain);
                    tweaksApplied++;

                    // 💀 Disable Hibernation (frees up disk space)
                    var hiberOff = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "powercfg",
                        Arguments = "/hibernate off",
                        Verb = "runas",
                        CreateNoWindow = true,
                        UseShellExecute = true
                    };
                    System.Diagnostics.Process.Start(hiberOff);
                    tweaksApplied++;

                    // 🚫 Disable Prefetch + Superfetch (via registry)
                    var prefetchKey = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(
                        @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters");
                    prefetchKey?.SetValue("EnablePrefetcher", 0, Microsoft.Win32.RegistryValueKind.DWord);
                    prefetchKey?.SetValue("EnableSuperfetch", 0, Microsoft.Win32.RegistryValueKind.DWord);
                    prefetchKey?.Close();
                    tweaksApplied++;


                    // 🎯 Processor scheduling tweak (Background services vs. Programs)
                    var schedKey = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\PriorityControl");
                    schedKey?.SetValue("Win32PrioritySeparation", 18, Microsoft.Win32.RegistryValueKind.DWord); // 18 = best for background services
                    schedKey?.Close();
                    tweaksApplied++;
                }
                catch { }
            }
                System.Windows.MessageBox.Show($"{tweaksApplied} system optimization(s) applied.", "Tweaks Complete", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
