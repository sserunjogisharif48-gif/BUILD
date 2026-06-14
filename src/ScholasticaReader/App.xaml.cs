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
            // var activation = new ActivationWindow();
            // if (activation.ShowDialog() != true)
            // {
            //     Shutdown();
            //     return;
            // }
            MessageBox.Show("Activation required (temporary bypass)");
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
