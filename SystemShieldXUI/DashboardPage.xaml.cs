using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SystemShieldXUI
{
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
        }

        // Handle the Start Quick Scan button click
        private void StartQuickScan_Click(object sender, RoutedEventArgs e)
        {
            var antivirusPage = new AntivirusPage();
            this.NavigationService?.Navigate(antivirusPage);

            // Small delay to let the page load visually
            Dispatcher.BeginInvoke(new Action(() =>
            {
                antivirusPage.RunQuickScan();
            }), DispatcherPriority.Background);
        }
    }
}
