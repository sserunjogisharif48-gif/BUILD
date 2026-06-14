using System;
using System.Threading.Tasks;
using System.Windows;
using ScholasticaReader.Services;
using ScholasticaReader.Views;
using ScholasticaReader.Helpers;

namespace ScholasticaReader;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        try
        {
            DatabaseService.Initialize();

            var hwid = HardwareID.GetUniqueHardwareId();
            var alreadyActivated = DatabaseService.IsHardwareIdAlreadyActivated(hwid);
            
            if (!alreadyActivated || SecurityService.IsWeeklyReAuthRequired())
            {
                // TODO: Replace with your actual activation window
                // For now, just show a message and continue (or replace with your ActivationWindow)
                MessageBox.Show("Activation would be required here (bypass for build).", "Info");
            }
            
            // Show main window
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Startup error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }
}
