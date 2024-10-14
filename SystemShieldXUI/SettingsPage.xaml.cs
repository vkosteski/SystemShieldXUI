using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace SystemShieldXUI
{
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            StartWithWindowsCheck.IsChecked = IsStartupEnabled();
            // Load other toggle states if you're saving them
        }

        private bool IsStartupEnabled()
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", false);
            return key?.GetValue("SystemShieldX") != null;
        }

        private void SaveStartupSetting()
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

            if (StartWithWindowsCheck.IsChecked == true)
            {
                string exePath = Process.GetCurrentProcess().MainModule.FileName;
                key.SetValue("SystemShieldX", $"\"{exePath}\"");
            }
            else
            {
                key.DeleteValue("SystemShieldX", false);
            }
        }

        private void ClearLogs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (Directory.Exists(logPath))
                {
                    Directory.Delete(logPath, true);
                    System.Windows.MessageBox.Show("Logs cleared.", "Success");
                }
                else
                {
                    System.Windows.MessageBox.Show("No logs found.");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error clearing logs: " + ex.Message);
            }
        }

        private void ResetSettings_Click(object sender, RoutedEventArgs e)
        {
            StartWithWindowsCheck.IsChecked = false;
            PlaySoundCheck.IsChecked = false;
            EnableNotificationsCheck.IsChecked = false;
            ShowAdvancedCheck.IsChecked = false;

            SaveStartupSetting();
            System.Windows.MessageBox.Show("Settings reset.");
        }

        private void OpenAppFolder_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", AppDomain.CurrentDomain.BaseDirectory);
        }

        private void CheckUpdates_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Update checking not yet implemented.", "Soon™");
        }

        private void StartWithWindowsCheck_Checked(object sender, RoutedEventArgs e)
        {
            SaveStartupSetting();
        }

        private void StartWithWindowsCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            SaveStartupSetting();
        }
    }
}
