using System;
using System.Windows;
using Microsoft.Win32;
using ScholasticaReader.Services;

namespace ScholasticaReader;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // Load library from local folder
        LoadLibrary();
    }

    private void LoadLibrary()
    {
        // Dummy: add sample books
        LibraryListBox.Items.Add("Sample.pdf");
        LibraryListBox.Items.Add("Sample.epub");
    }

    private void OpenBook_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog();
        dlg.Filter = "Book files|*.pdf;*.epub;*.mobi;*.cbz;*.txt";
        if (dlg.ShowDialog() == true)
        {
            // TODO: render file using PdfPig or EpubReader
            // BookViewer.Navigate(dlg.FileName);
            MessageBox.Show($"Opening book: {dlg.FileName}", "Book Loader");
        }
    }

    private void Exit_Click(object sender, RoutedEventArgs e) => Close();
    private void Library_Click(object sender, RoutedEventArgs e) => LoadLibrary();
    private void Notes_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Notes panel");
    private void About_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Scholastica Reader v1.0 - Secure Reader for Education");
}
