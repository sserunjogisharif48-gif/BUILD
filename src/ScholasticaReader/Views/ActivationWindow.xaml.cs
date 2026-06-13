using System;
using System.Windows;
using ScholasticaReader.Helpers;
using ScholasticaReader.Services;

namespace ScholasticaReader.Views;

public partial class ActivationWindow : Window
{
    public ActivationWindow()
    {
        InitializeComponent();
        HwidTextBox.Text = HardwareID.GetUniqueHardwareId();
    }

    private async void ActivateButton_Click(object sender, RoutedEventArgs e)
    {
        var code = CodeTextBox.Text.Trim();
        if (string.IsNullOrEmpty(code))
        {
            StatusText.Text = "Please enter activation code.";
            return;
        }

        var success = await LicenseManager.ActivateWithCode(code);
        if (success)
        {
            MessageBox.Show("Activation successful! The app will now open.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }
        else
        {
            StatusText.Text = "Invalid or already used activation code.";
        }
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}
