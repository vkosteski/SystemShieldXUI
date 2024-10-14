using System.Windows;
using System.Windows.Controls;

namespace SystemShieldXUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new DashboardPage()); // Default view
        }

        private void Nav_Dashboard(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashboardPage());
        }
        private void Nav_NetworkTools(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new NetworkToolsPage());
        }
        private void Nav_DiskAnalyzer(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SystemHealthAnalyzer());
        }
        private void Nav_Cleaner(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OptimizerPage());
        }

        private void Nav_Antivirus(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AntivirusPage());
        }

        private void Nav_Settings(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SettingsPage());
        }

        private void Nav_SystemInfo(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SystemInfoPage());
        }
    }
}
