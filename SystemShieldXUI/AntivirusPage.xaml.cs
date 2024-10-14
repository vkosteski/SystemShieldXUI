using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows;

namespace SystemShieldXUI
{
    public partial class AntivirusPage : Page
    {
        private List<string> maliciousHashes = new();
        private CancellationTokenSource cts;
        private bool isPaused = false;
        private bool isScanning = false;
        private IEnumerator<string> fileEnumerator;
        private int totalFiles = 0;
        private int threatsFound = 0;

        public AntivirusPage()
        {
            InitializeComponent();
            LoadMalwareSignatures();
        }

        private void LoadMalwareSignatures()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "malware-signatures.json");
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    maliciousHashes = JsonSerializer.Deserialize<List<string>>(json);
                }
                else
                {
                    ScanSummary.Text = "Signature file not found.";
                }
            }
            catch (Exception ex)
            {
                ScanSummary.Text = "Error loading signatures: " + ex.Message;
            }
        }

        private void StartScan_Click(object sender, RoutedEventArgs e)
        {
            if (!isScanning)
            {
                StartScanButton.Content = "Pause Scan";
                RunQuickScan();
            }
            else if (!isPaused)
            {
                isPaused = true;
                StartScanButton.Content = "Resume Scan";
                ScanStatusText.Text = "Paused";
            }
            else
            {
                isPaused = false;
                StartScanButton.Content = "Pause Scan";
                _ = ContinueScan();
            }
        }

        public async void RunQuickScan()
        {
            isScanning = true;
            isPaused = false;
            threatsFound = 0;
            ThreatCountText.Text = "0";
            ScanResults.Items.Clear();
            ScanSummary.Text = "";
            ScanProgressBar.Value = 0;
            ScanStatusText.Text = "Preparing scan...";
            StartScanAnimation();

            List<string> targets = new()
            {
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads",
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                @"C:\Windows\Temp",
                Path.GetTempPath()
            };

            string usersDir = @"C:\Users";
            if (Directory.Exists(usersDir))
            {
                foreach (var userDir in Directory.GetDirectories(usersDir))
                {
                    targets.Add(Path.Combine(userDir, @"AppData\Roaming"));
                    targets.Add(Path.Combine(userDir, @"AppData\Local"));
                    targets.Add(Path.Combine(userDir, @"Downloads"));
                    targets.Add(Path.Combine(userDir, @"Desktop"));
                }
            }

            List<string> allFiles = new();
            foreach (var dir in targets.Distinct())
            {
                try
                {
                    allFiles.AddRange(Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories));
                }
                catch { }
            }

            totalFiles = allFiles.Count;
            fileEnumerator = allFiles.GetEnumerator();
            ScanProgressBar.Maximum = totalFiles;
            cts = new CancellationTokenSource();

            await ContinueScan();
        }

        public async Task ContinueScan()
        {
            ScanStatusText.Text = "Scanning...";

            while (!isPaused && fileEnumerator.MoveNext())
            {
                string file = fileEnumerator.Current;
                try
                {
                    string hash = GetSHA256Hash(file);
                    if (maliciousHashes.Contains(hash))
                    {
                        threatsFound++;
                        ScanResults.Items.Add(file);
                        ThreatCountText.Text = threatsFound.ToString();
                    }
                }
                catch { }

                ScanProgressBar.Value++;
                ScanStatusText.Text = $"Scanning file {ScanProgressBar.Value} of {totalFiles}";
                await Task.Delay(1); // Keep UI responsive
            }

            if (ScanProgressBar.Value >= totalFiles)
            {
                isScanning = false;
                isPaused = false;
                StopScanAnimation();
                StartScanButton.Content = "Start Scan";
                ScanStatusText.Text = "Scan complete";
                ScanSummary.Text = $"Scan complete: {totalFiles} files scanned | {threatsFound} threats found.";
            }
        }

        private void StartScanAnimation()
        {
            var rotate = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = TimeSpan.FromSeconds(1),
                RepeatBehavior = RepeatBehavior.Forever
            };

            if (ScanIcon.RenderTransform is RotateTransform iconRotate)
            {
                iconRotate.BeginAnimation(RotateTransform.AngleProperty, rotate);
            }
        }

        private void StopScanAnimation()
        {
            if (ScanIcon.RenderTransform is RotateTransform iconRotate)
            {
                iconRotate.BeginAnimation(RotateTransform.AngleProperty, null);
            }
        }

        private string GetSHA256Hash(string filePath)
        {
            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(filePath);
            byte[] hashBytes = sha256.ComputeHash(stream);
            StringBuilder sb = new();
            foreach (byte b in hashBytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        private void QuarantineThreats_Click(object sender, RoutedEventArgs e)
        {
            if (ScanResults.Items.Count == 0)
            {
                System.Windows.MessageBox.Show("No threats to quarantine.", "Quarantine", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string quarantineFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Quarantine");
            if (!Directory.Exists(quarantineFolder))
                Directory.CreateDirectory(quarantineFolder);

            int success = 0;
            int failed = 0;

            foreach (string filePath in ScanResults.Items)
            {
                try
                {
                    string fileName = Path.GetFileName(filePath);
                    string destPath = Path.Combine(quarantineFolder, fileName);

                    int count = 1;
                    while (File.Exists(destPath))
                    {
                        string newName = $"{Path.GetFileNameWithoutExtension(fileName)}_{count}{Path.GetExtension(fileName)}";
                        destPath = Path.Combine(quarantineFolder, newName);
                        count++;
                    }

                    File.Move(filePath, destPath);
                    success++;
                }
                catch
                {
                    failed++;
                }
            }

         System.Windows.MessageBox.Show(
                $"Quarantine complete.\n\nFiles moved: {success}\nFailed: {failed}",
                "Quarantine Summary",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            ScanResults.Items.Clear();
            ThreatCountText.Text = "0";
            ScanSummary.Text += $" | {success} quarantined.";
        }
    }
}
