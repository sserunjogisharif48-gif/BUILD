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
                // Show activation window
                var activation = new ActivationWindow();
                if (activation.ShowDialog() != true)
                {
                    Shutdown();
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Startup error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }
}
