using System.Configuration;
using System.Data;
using System.Windows;

namespace BhavaniTech.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        this.DispatcherUnhandledException += (s, args) =>
        {
            try
            {
                string log = $"[{System.DateTime.Now:yyyy-MM-dd HH:mm:ss}] DispatcherUnhandledException:\n{args.Exception}\n\n";
                System.IO.File.AppendAllText("crash.log", log);
                MessageBox.Show($"Application Error: {args.Exception.Message}\n\nDetails saved to crash.log", 
                    "Bhavani Technology", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch { }
            args.Handled = true;
        };

        System.AppDomain.CurrentDomain.UnhandledException += (s, args) =>
        {
            try
            {
                string log = $"[{System.DateTime.Now:yyyy-MM-dd HH:mm:ss}] CurrentDomain_UnhandledException:\n{args.ExceptionObject}\n\n";
                System.IO.File.AppendAllText("crash.log", log);
                MessageBox.Show($"Fatal Error: {args.ExceptionObject}\n\nDetails saved to crash.log", 
                    "Bhavani Technology Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch { }
        };

        base.OnStartup(e);
    }
}

