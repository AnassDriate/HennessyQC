using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Serilog;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configure Serilog to log to a file and console (optional)
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()  // Optional for console logging
                .WriteTo.File("logs/logfile.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Application started!");

        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Ensure to flush and close loggers on exit
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}
