using System.Windows;

using R3name.Models;
using R3name.ViewModels;
using R3name.Views;

namespace R3name;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private MainViewModel _viewModel;
    public App()
    {
        DispatcherUnhandledException += (sender, e) =>
        {
            MessageBox.Show(e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        };
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var args = e.Args;


        MainWindow = new MainWindow();
        _viewModel = MainWindow.DataContext as MainViewModel;
        WindowService.CreateInstance(MainWindow);

        MainWindow.Show();

        
        _viewModel.GoWithArguments(args);
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
        _viewModel.SaveSettings();
    }
}