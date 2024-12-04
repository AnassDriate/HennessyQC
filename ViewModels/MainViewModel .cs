using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Serilog;
using System.Timers;
using System.Runtime.Remoting.Messaging;
using System.Windows;
using labid;
using System.Threading;
using System.Xml.Linq;
using WpfApp1.Services;


namespace WpfApp1.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private int _goodProductCount;
        private int _badProductCount;
        private Thread _workerThread; // Thread reference
        private bool _isRunning;      // Status flag
        BearsReader reader = new BearsReader();


        // Command for Reset button
        public ICommand ResetCommand { get; }

        // Command for Settings button
        public ICommand StartStopCommand { get; }

        public ICommand AboutCommand { get; }

        public MainViewModel()
        {
            // Initialize default values
            GoodProductCount = 0;
            BadProductCount = 0;

            // Commands
            ResetCommand = new RelayCommand(ResetCounters);
            StartStopCommand = new RelayCommand(StartStopb);
            AboutCommand = new RelayCommand(OpenAbout);


            try
            {
                reader.Connect();
                Log.Information("Reader connected , application started ....");
            }
            catch (Exception ex)
            {
                //Log.Error("RFID Reader connection > False " + ex.Message);
                //MessageBox.Show("RFID Reader connection > False", "Exit Message", MessageBoxButton.OK, MessageBoxImage.Information);
                //Application.Current.Shutdown();
            }
        }

        // Product Counter for bad products
        public int GoodProductCount
        {
            get { return _goodProductCount; }
            set
            {
                _goodProductCount = value;
                OnPropertyChanged(nameof(GoodProductCount));
            }
        }


        // Product Counter for bad products
        public int BadProductCount
        {
            get { return _badProductCount; }
            set
            {
                _badProductCount = value;
                OnPropertyChanged(nameof(BadProductCount));
            }
        }

        private string _startStopButtonText = "Start";
        public string StartStopButtonText
        {
            get => _startStopButtonText;
            set
            {
                _startStopButtonText = value;
                OnPropertyChanged(nameof(StartStopButtonText));
            }
        }


        private void StartStopb(object obj)
        {
            if (!_isRunning)
            {
                // Start the thread
                _isRunning = true;
                StartStopButtonText = "Stop"; // Update button text

                _workerThread = new Thread(QualityCheck)
                {
                    IsBackground = true // Ensure the thread stops when the app closes
                };
                _workerThread.Start();
            }
            else
            {
                // Stop the thread
                _isRunning = false;
                StartStopButtonText = "Start"; // Update button text

                if (_workerThread != null && _workerThread.IsAlive)
                {
                    _workerThread.Join(); // Wait for the thread to finish gracefully
                }
            }
        }

        private void QualityCheck()
        {
            while (_isRunning)
            {
                try
                {
                    // Call the ReadUrl function to get the URL
                    string url = Utilities.ReadUrl(reader); // Ensure 'reader' is properly initialized
                    if (string.IsNullOrEmpty(url))
                    {
                        //Log.Information("No URL read or invalid URL, Dead tag");
                        continue; // Skip to the next iteration if the URL is invalid
                    }

                    // Check if the product is closed
                    if (Utilities.CheckIfClosed(url))
                    {
                        GoodProductCount++;


                        for (int i = 0; i < 3; i++)
                        {
                            reader.Mifare.beep();
                        }
                    }
                    else
                    {
                        BadProductCount++;
                        reader.Mifare.beep();
                    }

                    // Simulate processing delay (optional)
                    Thread.Sleep(500); // Adjust the delay as needed
                }
                catch (Exception ex)
                {
                    Log.Error($"An error occurred in QualityCheck: {ex.Message}");
                }
            }
            Log.Information("Thread stopped.");
        }

        // Reset counters for products
        private void ResetCounters(object obj)
        {
            Log.Information("Counters reseted...");
            BadProductCount = 0;
            GoodProductCount = 0;
        }
        private void OpenAbout(object obj)
        {
            string aboutMessage = "Hennessy CTT Closures Quality Control Application\n\n" +
                                  "Version: 1.0.0\n" +
                                  "Developer: Anass Driate\n" +
                                  "Contact: driateanass@gmail.com\n\n" +
                                  "This application is designed for quality control of Hennessy CTT closures. " +
                                  "It monitors and controls the permanent and current closed status (CC) of closures, ensuring high standards.";
            MessageBox.Show(aboutMessage, "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
