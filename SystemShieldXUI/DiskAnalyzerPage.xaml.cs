using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

// Resolve namespace conflicts:
using IOPath = System.IO.Path;
using MediaColor = System.Windows.Media.Color;

namespace SystemShieldXUI
{
    public partial class SystemHealthAnalyzer : Page
    {
        public SystemHealthAnalyzer()
        {
            InitializeComponent();
            AnalyzeSystemDrive();
        }

        public class FolderUsage
        {
            public string Name { get; set; }
            public long Size { get; set; }
            public string SizeFormatted { get; set; }
        }

        private void AnalyzeSystemDrive()
        {
            string drive = IOPath.GetPathRoot(Environment.SystemDirectory);
            var driveInfo = new DriveInfo(drive);

            long totalBytes = driveInfo.TotalSize;
            long freeBytes = driveInfo.TotalFreeSpace;
            long usedBytes = totalBytes - freeBytes;

            DriveLetterText.Text = driveInfo.Name;
            UsedSpaceText.Text = FormatSize(usedBytes);
            FreeSpaceText.Text = FormatSize(freeBytes);
            TotalSpaceText.Text = FormatSize(totalBytes);

            double usedPercent = usedBytes / (double)totalBytes;
            UsageBar.Width = usedPercent * 820; // full width of usage bar container

            // Health color logic
            if (usedPercent >= 0.9)
            {
                UsageBar.Fill = new SolidColorBrush(MediaColor.FromRgb(255, 80, 80)); // 🔴 Red
                HealthStatusText.Text = "⚠️ CRITICAL: Your system drive is almost full.";
                HealthStatusText.Foreground = System.Windows.Media.Brushes.Red;
            }
            else if (usedPercent >= 0.8)
            {
                UsageBar.Fill = new SolidColorBrush(MediaColor.FromRgb(255, 160, 0)); // 🟠 Orange
                HealthStatusText.Text = "⚠️ WARNING: Your system drive is getting full.";
                HealthStatusText.Foreground = System.Windows.Media.Brushes.Orange;
            }
            else
            {
                UsageBar.Fill = new SolidColorBrush(MediaColor.FromRgb(0, 120, 215)); // 🟦 Blue
                HealthStatusText.Text = "✅ Your system drive looks healthy.";
                HealthStatusText.Foreground = System.Windows.Media.Brushes.LimeGreen;
            }

            // Analyze top-level folders in C:\
            var folders = Directory.GetDirectories(drive)
                .Select(path => new FolderUsage
                {
                    Name = IOPath.GetFileName(path),
                    Size = GetFolderSize(path)
                })
                .OrderByDescending(x => x.Size)
                .Take(10)
                .ToList();

            foreach (var folder in folders)
            {
                folder.SizeFormatted = FormatSize(folder.Size);
                TopUsageList.Items.Add(folder);
            }
        }

        private long GetFolderSize(string path)
        {
            long size = 0;
            try
            {
                foreach (string file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                {
                    try { size += new FileInfo(file).Length; } catch { }
                }
            }
            catch { } // skip inaccessible directories
            return size;
        }

        private string FormatSize(long bytes)
        {
            if (bytes >= 1_000_000_000) return $"{bytes / 1_000_000_000.0:F2} GB";
            if (bytes >= 1_000_000) return $"{bytes / 1_000_000.0:F2} MB";
            if (bytes >= 1_000) return $"{bytes / 1_000.0:F2} KB";
            return $"{bytes} B";
        }
    }
}
